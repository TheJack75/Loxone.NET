﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Loxone.Client {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Loxone.Client.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to The color must be specified in format &apos;#RRGGBB&apos;..
        /// </summary>
        internal static string ColorConverter_InvalidFormat {
            get {
                return ResourceManager.GetString("ColorConverter_InvalidFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hex string format is invalid..
        /// </summary>
        internal static string HexConverter_BadFormat {
            get {
                return ResourceManager.GetString("HexConverter_BadFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Miniserver command failed with status code {0}..
        /// </summary>
        internal static string MiniserverCommandException_MessageFmt {
            get {
                return ResourceManager.GetString("MiniserverCommandException_MessageFmt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Property &apos;{0}&apos; must be set before opening the connection..
        /// </summary>
        internal static string MiniserverConnection_MustBeSetBeforeOpenFmt {
            get {
                return ResourceManager.GetString("MiniserverConnection_MustBeSetBeforeOpenFmt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Message received from Miniserver has invalid format..
        /// </summary>
        internal static string MiniserverTransportException_Message {
            get {
                return ResourceManager.GetString("MiniserverTransportException_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Argument must be of type Uuid..
        /// </summary>
        internal static string Uuid_ArgMustBeUuid {
            get {
                return ResourceManager.GetString("Uuid_ArgMustBeUuid", resourceCulture);
            }
        }
    }
}
