﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShagBot {
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
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ShagBot.Resource", typeof(Resource).Assembly);
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
        ///   Looks up a localized string similar to This command can only be used through a DM to the bot.
        /// </summary>
        internal static string AdminHelpRemarks {
            get {
                return ResourceManager.GetString("AdminHelpRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Returns a list of available admin commands and their parameters, or if specified, details about a specific admin command..
        /// </summary>
        internal static string AdminHelpSummary {
            get {
                return ResourceManager.GetString("AdminHelpSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command can only be used through a DM to the bot.
        /// </summary>
        internal static string ApproveChannelRemarks {
            get {
                return ResourceManager.GetString("ApproveChannelRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command will approve the currently pending request that is requesting the specified channel name.
        /// </summary>
        internal static string ApproveChannelSummary {
            get {
                return ResourceManager.GetString("ApproveChannelSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command can only be used through a DM to the bot.
        /// </summary>
        internal static string ApproveEmojiRemarks {
            get {
                return ResourceManager.GetString("ApproveEmojiRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command will approve the currently pending emoji that is requesting the specified shortcut.
        /// </summary>
        internal static string ApproveEmojiSummary {
            get {
                return ResourceManager.GetString("ApproveEmojiSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The command will fail if a role with the specified name already exists..
        /// </summary>
        internal static string CreateGroupRemarks {
            get {
                return ResourceManager.GetString("CreateGroupRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creates a new mentionable group with the name given by the roleName parameter..
        /// </summary>
        internal static string CreateGroupSummary {
            get {
                return ResourceManager.GetString("CreateGroupSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command will not allow you to delete roles that are not custom mention groups. This command will also not allow you to delete a custom mention group if there are users currently part of the group..
        /// </summary>
        internal static string DeleteGroupRemarks {
            get {
                return ResourceManager.GetString("DeleteGroupRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command will delete the specified custom mention group..
        /// </summary>
        internal static string DeleteGroupSummary {
            get {
                return ResourceManager.GetString("DeleteGroupSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error will be returned if the provided values aren&apos;t integers, or if the minimum value exceeds the maximum value..
        /// </summary>
        internal static string DiceRemarks {
            get {
                return ResourceManager.GetString("DiceRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command will return a random integer between the minimum and maximum values inclusively..
        /// </summary>
        internal static string DiceSummary {
            get {
                return ResourceManager.GetString("DiceSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Returns a list of available commands and their parameters, or if specified, details about a specific command..
        /// </summary>
        internal static string HelpSummary {
            get {
                return ResourceManager.GetString("HelpSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The command will fail if you try to join an unjoinable group..
        /// </summary>
        internal static string JoinGroupRemarks {
            get {
                return ResourceManager.GetString("JoinGroupRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use to add yourself to an existing mentionable group..
        /// </summary>
        internal static string JoinGroupSummary {
            get {
                return ResourceManager.GetString("JoinGroupSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use to leave a mentionable group that you are currently part of..
        /// </summary>
        internal static string LeaveGroupSummary {
            get {
                return ResourceManager.GetString("LeaveGroupSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The command will fail if you are not part of the group, or if you do not have permission to leave the specified role..
        /// </summary>
        internal static string LeaveGrroupRemarks {
            get {
                return ResourceManager.GetString("LeaveGrroupRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Displays a list of all the available mentionable groups that you can join..
        /// </summary>
        internal static string ListGroupsSummary {
            get {
                return ResourceManager.GetString("ListGroupsSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The command will fail if the specified mention group is not a custom group that was created with the creategroup command..
        /// </summary>
        internal static string ListMembersRemarks {
            get {
                return ResourceManager.GetString("ListMembersRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command lists all the members in the specified mention group..
        /// </summary>
        internal static string ListMembersSummary {
            get {
                return ResourceManager.GetString("ListMembersSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command can only be used through a DM to the bot.
        /// </summary>
        internal static string ListPendingChannelsRemarks {
            get {
                return ResourceManager.GetString("ListPendingChannelsRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command will list the currently pending channels awaiting approval.
        /// </summary>
        internal static string ListPendingChannelsSummary {
            get {
                return ResourceManager.GetString("ListPendingChannelsSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command can only be used through a DM to the bot.
        /// </summary>
        internal static string ListPendingEmojisRemarks {
            get {
                return ResourceManager.GetString("ListPendingEmojisRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command will list the currently pending emojis awaiting approval.
        /// </summary>
        internal static string ListPendingEmojisSummary {
            get {
                return ResourceManager.GetString("ListPendingEmojisSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command can only be used through a DM to the bot.
        /// </summary>
        internal static string RejectChannelRemarks {
            get {
                return ResourceManager.GetString("RejectChannelRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command will reject the currently pending request that is requesting the specified channel name, and will then ping the user indicating why the channel was rejected.
        /// </summary>
        internal static string RejectChannelSummary {
            get {
                return ResourceManager.GetString("RejectChannelSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command can only be used through a DM to the bot.
        /// </summary>
        internal static string RejectEmojiRemarks {
            get {
                return ResourceManager.GetString("RejectEmojiRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command will reject the currently pending emoji that is requesting the specified shortcut, and will then ping the user indicating why the emoji is rejected.
        /// </summary>
        internal static string RejectEmojiSummary {
            get {
                return ResourceManager.GetString("RejectEmojiSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 1) This command can be used in any channel (it is not limited only to the Bot Channel).\r\n\r\n2) This command will only remove 10 messages at a time at most..
        /// </summary>
        internal static string RemoveLastMessagesRemarks {
            get {
                return ResourceManager.GetString("RemoveLastMessagesRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command will remove the last X number of messages from the current channel..
        /// </summary>
        internal static string RemoveLastMessagesSummary {
            get {
                return ResourceManager.GetString("RemoveLastMessagesSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 1)  Channels can only have names containing alphanumeric characters, dashes or underscores. \r\n2) You cannot specify a channel name that exists, or is already being requested for use. \r\n3) You may optionally specify which category you would like to add the text channel under. The text channel will automatically inherit all permissions associated with this category. \r\n4) If you are getting the error &quot;Channel name not found.&quot;, it is because the specified category does not exist. You may also see this err [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RequestChannelRemarks {
            get {
                return ResourceManager.GetString("RequestChannelRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use this command to request a text channel to be added to the server. Optionally specify a category to put the channel in..
        /// </summary>
        internal static string RequestChannelSummary {
            get {
                return ResourceManager.GetString("RequestChannelSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 1) You can only use this command with either a url of the image, or a single image attachment (cannot use both).\r\n\r\n2) To use the command with an image upload, first upload the image as an attachment to the bot channel. When it asks you type in an optional message with the attachment, enter the command.\r\n\r\n3) To use the command with a url of the image, you must ensure the url is a direct url to the image.\r\n\r\n4) If you upload an emoji with a shortcut name matching one that is already pending, the [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RequestEmojiRemarks {
            get {
                return ResourceManager.GetString("RequestEmojiRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use this command to request an emoji to be added to the server. Emojis can either be provided by direct upload, or a direct url to an image..
        /// </summary>
        internal static string RequestEmojiSummary {
            get {
                return ResourceManager.GetString("RequestEmojiSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This command can only be used through a DM to the bot.
        /// </summary>
        internal static string SayRemarks {
            get {
                return ResourceManager.GetString("SayRemarks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use this command to say the specified message into the bot channel..
        /// </summary>
        internal static string SaySummary {
            get {
                return ResourceManager.GetString("SaySummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The command will randomly generate a sudoku puzzle image.
        /// </summary>
        internal static string SudokuSummary {
            get {
                return ResourceManager.GetString("SudokuSummary", resourceCulture);
            }
        }
    }
}
