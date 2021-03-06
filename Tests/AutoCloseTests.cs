﻿namespace Tests
{
	using Newtonsoft.Json;
	using OctoHook;
	using Octokit;
	using Octokit.Events;
	using Octokit.Internal;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using Xunit;

	public class AutoCloseTests
	{
		static readonly Credentials credentials = TestCredentials.Create();

		static AutoCloseTests()
		{
			AutoMapper.Mapper.CreateMap<Repository, PushEvent.RepositoryInfo>();
		}

		[Fact]
		public void when_parsing_issue_message_then_can_detect_close_verbs()
		{
			var regex = new Regex(@"(close[s|d]?|fix(es|ed)?|resolve[s|d]?)\s\#\d+",
				RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);

			Assert.True(regex.IsMatch("Close #123, #345"));
			Assert.True(regex.IsMatch("Closes #123, #345"));
			Assert.True(regex.IsMatch("closed #123, #345"));
			Assert.True(regex.IsMatch("fixes #123, #345"));
			Assert.True(regex.IsMatch("Fixed #123, #345"));
			Assert.True(regex.IsMatch("fix #123, #345"));
			Assert.True(regex.IsMatch("resolve #123, #345"));
			Assert.True(regex.IsMatch("resolves #123, #345"));
			Assert.True(regex.IsMatch("resolved #123, #345"));
		}

		[Fact]
		public async Task when_processing_push_then_auto_closes_issue()
		{
			var github = new GitHubClient(new ProductHeaderValue("kzu-client"), new InMemoryCredentialStore(credentials));
			var repository = await github.Repository.Get("kzu", "sandbox");
			var user = await github.User.Current();

			var task = await github.Issue.Create(
				"kzu", "sandbox", new NewIssue("Task that should auto-close from commit")
				{
					Labels = { "Task" },
				});

			var hook = new AutoClose(github);

			await hook.ProcessAsync(new PushEvent
			{
				Commits = new[]
				{ 
					new PushEvent.CommitInfo
					{
						Message = "Closes #" + task.Number,
						Sha = "ae257bb398c8aa3293b70c7495ae43033a5f0698"
					},
				},
				HeadCommit = new PushEvent.CommitInfo
				{
					Message = "Closes #" + task.Number,
					Sha = "ae257bb398c8aa3293b70c7495ae43033a5f0698"
				},
				Repository = AutoMapper.Mapper.Map<PushEvent.RepositoryInfo>(repository)
			});

			var updated = await github.Issue.Get("kzu", "sandbox", task.Number);

			Assert.Equal(ItemState.Closed, updated.State);
		}

		[Fact]
		public async Task when_processing_push_then_preserves_labels()
		{
			var github = new GitHubClient(new ProductHeaderValue("kzu-client"), new InMemoryCredentialStore(credentials));
			var repository = await github.Repository.Get("kzu", "sandbox");
			var user = await github.User.Current();

			var task = await github.Issue.Create(
				"kzu", "sandbox", new NewIssue("Task that should auto-close from commit")
				{
					Labels = { "Task" },
				});

			var hook = new AutoClose(github);

			await hook.ProcessAsync(new PushEvent
			{
				Commits = new[]
				{ 
					new PushEvent.CommitInfo
					{
						Message = "Closes #" + task.Number,
						Sha = "ae257bb398c8aa3293b70c7495ae43033a5f0698"
					},
				},
				HeadCommit = new PushEvent.CommitInfo
				{
					Message = "Closes #" + task.Number,
					Sha = "ae257bb398c8aa3293b70c7495ae43033a5f0698"
				},
				Repository = AutoMapper.Mapper.Map<PushEvent.RepositoryInfo>(repository)
			});

			var updated = await github.Issue.Get("kzu", "sandbox", task.Number);

			Assert.Equal(1, updated.Labels.Count);
			Assert.Equal("Task", updated.Labels.Select(l => l.Name).First());
		}

		[Fact]
		public async Task when_processing_push_then_clears_asignee()
		{
			var github = new GitHubClient(new ProductHeaderValue("kzu-client"), new InMemoryCredentialStore(credentials));
			var repository = await github.Repository.Get("kzu", "sandbox");
			var user = await github.User.Current();

			var task = await github.Issue.Create(
				"kzu", "sandbox", new NewIssue("Task that should auto-close from commit")
				{
					Labels = { "Task" },
					Assignee = "kzu",
				});

			var hook = new AutoClose(github);

			await hook.ProcessAsync(new PushEvent
			{
				Commits = new[]
				{ 
					new PushEvent.CommitInfo
					{
						Message = "Closes #" + task.Number,
						Sha = "ae257bb398c8aa3293b70c7495ae43033a5f0698"
					},
				},
				HeadCommit = new PushEvent.CommitInfo
				{
					Message = "Closes #" + task.Number,
					Sha = "ae257bb398c8aa3293b70c7495ae43033a5f0698"
				},
				Repository = AutoMapper.Mapper.Map<PushEvent.RepositoryInfo>(repository)
			});

			var updated = await github.Issue.Get("kzu", "sandbox", task.Number);

			Assert.Null(updated.Assignee);
		}

		[Fact]
		public async Task when_processing_push_multiple_commits_then_auto_closes_issues()
		{
			var github = new GitHubClient(new ProductHeaderValue("kzu-client"), new InMemoryCredentialStore(credentials));
			var repository = await github.Repository.Get("kzu", "sandbox");
			var user = await github.User.Current();

			var task1 = await github.Issue.Create(
				"kzu", "sandbox", new NewIssue("Task1 that should auto-close from commit")
				{
					Labels = { "Task" },
				});
			var task2 = await github.Issue.Create(
				"kzu", "sandbox", new NewIssue("Task2 that should auto-close from commit")
				{
					Labels = { "Task" },
				});

			var hook = new AutoClose(github);

			await hook.ProcessAsync(new PushEvent
			{
				Commits = new[]
				{ 
					new PushEvent.CommitInfo
					{
						Message = "Closes #" + task1.Number,
						Sha = "ae257bb398c8aa3293b70c7495ae43033a5f0698"
					},
					new PushEvent.CommitInfo
					{
						Message = "Closes #" + task2.Number, 
						Sha = "a04dbe98b61ca89cd026b5773c9026bc4569854d",
					},
				},
				HeadCommit = new PushEvent.CommitInfo
				{
					Message = "Closes #" + task1.Number,
					Sha = "ae257bb398c8aa3293b70c7495ae43033a5f0698"
				},
				Repository = AutoMapper.Mapper.Map<PushEvent.RepositoryInfo>(repository)
			});

			var updated1 = await github.Issue.Get("kzu", "sandbox", task1.Number);
			var updated2 = await github.Issue.Get("kzu", "sandbox", task2.Number);

			Assert.Equal(ItemState.Closed, updated1.State);
			Assert.Equal(ItemState.Closed, updated2.State);
		}

		[Fact]
		public async Task when_processing_push_then_auto_closes_multiple_issues_from_single_message()
		{
			var github = new GitHubClient(new ProductHeaderValue("kzu-client"), new InMemoryCredentialStore(credentials));
			var repository = await github.Repository.Get("kzu", "sandbox");
			var user = await github.User.Current();

			var task1 = await github.Issue.Create(
				"kzu", "sandbox", new NewIssue("Task1 that should auto-close from commit")
				{
					Labels = { "Task" },
				});
			var task2 = await github.Issue.Create(
				"kzu", "sandbox", new NewIssue("Task2 that should auto-close from commit")
				{
					Labels = { "Task" },
				});

			var hook = new AutoClose(github);

			await hook.ProcessAsync(new PushEvent
			{
				Commits = new[]
				{ 
					new PushEvent.CommitInfo
					{
						Message = "Closes #" + task1.Number + " and #" + task2.Number,
						Sha = "ae257bb398c8aa3293b70c7495ae43033a5f0698"
					},
				},
				HeadCommit = new PushEvent.CommitInfo
				{
					Message = "Closes #" + task1.Number + " and #" + task2.Number,
					Sha = "ae257bb398c8aa3293b70c7495ae43033a5f0698"
				},
				Repository = AutoMapper.Mapper.Map<PushEvent.RepositoryInfo>(repository)
			});

			var updated1 = await github.Issue.Get("kzu", "sandbox", task1.Number);
			var updated2 = await github.Issue.Get("kzu", "sandbox", task2.Number);

			Assert.Equal(ItemState.Closed, updated1.State);
			Assert.Equal(ItemState.Closed, updated2.State);
		}
	}
}
