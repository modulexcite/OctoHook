﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.35312
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OctoHook.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("OctoHook.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///&lt;!-- This section was generated by OctoHook v{version} --&gt;
        ///&lt;a href=&quot;{wiki}&quot; title=&quot;{title}&quot;&gt;&lt;img src=&quot;http://goo.gl/q05sAm&quot; /&gt;&lt;/a&gt;
        ///
        ///&gt; {note}
        ///.
        /// </summary>
        internal static string FormatHeader {
            get {
                return ResourceManager.GetString("FormatHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to - [{check}] {issue} {title}.
        /// </summary>
        internal static string FormatTask {
            get {
                return ResourceManager.GetString("FormatTask", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This task list is updated automatically when the referenced issues are closed or reopened..
        /// </summary>
        internal static string Note {
            get {
                return ResourceManager.GetString("Note", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Learn more about OctoHook&apos;s AutoTask.
        /// </summary>
        internal static string Title {
            get {
                return ResourceManager.GetString("Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Created new task list and added new link: {taskLink}..
        /// </summary>
        internal static string Trace_AddedLinkInNewList {
            get {
                return ResourceManager.GetString("Trace_AddedLinkInNewList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Processing found link to {linkedOwner}/{linkedRepo}#{linkedNumer} from body of {issueOwner}/{issueRepo}#{issueNumber}..
        /// </summary>
        internal static string Trace_FoundLinkInBody {
            get {
                return ResourceManager.GetString("Trace_FoundLinkInBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inserted new link in existing task list: {taskLink}..
        /// </summary>
        internal static string Trace_InsertedLinkInExistingList {
            get {
                return ResourceManager.GetString("Trace_InsertedLinkInExistingList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Found existing link in task list. Updating with entry: {taskLink}..
        /// </summary>
        internal static string Trace_UpdatedExistingLink {
            get {
                return ResourceManager.GetString("Trace_UpdatedExistingLink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http://goo.gl/iB4ZFR.
        /// </summary>
        internal static string Wiki {
            get {
                return ResourceManager.GetString("Wiki", resourceCulture);
            }
        }
    }
}