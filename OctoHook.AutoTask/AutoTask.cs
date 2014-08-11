﻿namespace OctoHook
{
	using Octokit;
	using Octokit.Events;
	using System;
	using System.Text.RegularExpressions;
	using System.Linq;
	using System.Collections.ObjectModel;
	using OctoHook.CommonComposition;
	using System.Threading.Tasks;
	using OctoHook.Diagnostics;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Reflection;
	using OctoHook.Properties;

	[Component]
	public class AutoTask : IOctoJob<IssuesEvent>
	{
		internal const string SectionBegin = "<!-- Begin OctoHook.AutoTask -->";
		internal const string SectionEnd = "<!-- End OctoHook.AutoTask -->";

		internal static readonly string header = Strings.FormatHeader(ThisAssembly.InformationalVersion,
			Strings.Wiki, Strings.Title, Strings.Note);
		static readonly string taskLinkExpr = @"-\s\[(\s|x|X)\]\s{0}.+$";

		static readonly ITracer tracer = Tracer.Get<AutoTask>();
		static readonly Regex issueLinkExpr = new Regex(@"(?<task>-\s\[(\s|x|X)\]\s)?((?<owner>\w+)/(?<repo>\w+))?#(?<number>\d+)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		static readonly Regex headerExpr = new Regex(@"<!-- This section was generated by OctoHook v[^\s]+ -->", RegexOptions.Compiled);

		private IGitHubClient github;

		public AutoTask(IGitHubClient github)
		{
			this.github = github;
		}

		public async Task ProcessAsync(IssuesEvent @event)
		{
			// Need to retrieve the full issue, since the event only contains the title
			var issue = await github.Issue.Get(@event.Repository.Owner.Login, @event.Repository.Name, @event.Issue.Number);

			foreach (var link in issueLinkExpr.Matches(issue.Body)
				.OfType<Match>()
                // Skip the links that are incoming links from a task list 
                // themselves (like the ones we generate!)
				.Where(m => m.Success && !m.Groups["task"].Success)
				.Select(m => new
				{
					// We support cross-repo links too, see https://github.com/blog/967-github-secrets
					Owner = string.IsNullOrEmpty(m.Groups["owner"].Value) ? @event.Repository.Owner.Login : m.Groups["owner"].Value,
					Repo = string.IsNullOrEmpty(m.Groups["repo"].Value) ? @event.Repository.Name : m.Groups["repo"].Value,
					Number = int.Parse(m.Groups["number"].Value),
				}))
			{
				try
				{
					tracer.Verbose(Strings.Trace.FoundLinkInBody(
						link.Owner, link.Repo, link.Number, 
						@event.Repository.Owner.Login, @event.Repository.Name, @event.Issue.Number));

					var linked = await github.Issue.Get(link.Owner, link.Repo, link.Number);
					// Simplifies code later.
					if (linked.Body == null)
						linked.Body = "";

					// If the repos don't match, then the foreign link to our issue would need to 
					// be a cross-repo link instead.
					var expectedLink =
						(link.Owner != @event.Repository.Owner.Login ||
						link.Repo != @event.Repository.Name) ?
						@event.Repository.Owner.Login + "/" + @event.Repository.Name + "#" + @event.Issue.Number :
						// Otherwise, it will be a simple link
						"#" + @event.Issue.Number;

					var taskLink = Strings.FormatTask(@event.Issue.State == ItemState.Closed ? "x" : " ", expectedLink, @event.Issue.Title).Trim();
					var taskLinkRegex = new Regex(string.Format(CultureInfo.InvariantCulture, taskLinkExpr, expectedLink), RegexOptions.Multiline);

                    var existingMatch = taskLinkRegex.Match(linked.Body);
                    if (existingMatch.Success && existingMatch.Value.Trim() == taskLink)
                        return;

					var newBody = taskLinkRegex.Replace(linked.Body, taskLink);

					// If the new body isn't different, we haven't found an existing link, so we need to add 
					// our header (if non-existent) and the task link.
					if (linked.Body == newBody)
					{
						var indexOfBegin = newBody.IndexOf(SectionBegin);
						if (indexOfBegin == -1)
						{
							// Only add header if we didn't find the begin marker. 
							// This allows deletion of our header and note, just in case 
							// someone prefers not to publizice OctoHook :(
							var headerMatch = headerExpr.Match(newBody);
							if (!headerMatch.Success)
								newBody += header;

							newBody += SectionBegin;
						}

						var indexOfEnd = newBody.IndexOf(SectionEnd);
						if (indexOfEnd == -1)
						{
							// Simply append the link and the section end.
							newBody += Environment.NewLine + taskLink + Environment.NewLine + SectionEnd;
							// Trace this as a new task list scenario, since the non-existing end usually 
							// is accompanied by a non-existing begin too.
							tracer.Info(Strings.Trace.AddedLinkInNewList(taskLink));
						}
						else
						{
							// Otherwise, insert it before the end section, with a new line at the end.
							newBody = newBody.Insert(indexOfEnd, taskLink + Environment.NewLine);
							tracer.Info(Strings.Trace.InsertedLinkInExistingList(taskLink));
						}
					}
					else
					{
						tracer.Info(Strings.Trace.UpdatedExistingLink(taskLink));
					}

					// Finally, update the referenced task body.
					await github.Issue.Update(link.Owner, link.Repo, link.Number, new IssueUpdate
						{
							Body = newBody
						});
				}
				catch (NotFoundException)
				{
					// It may be a link to a bug/issue in another system.
				}
				catch (Exception ex)
				{
					tracer.Error("Failed to process issue.", ex);
                    throw;
				}
			}
		}
	}
}
