﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Indy.Sockets.Protocols {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ResourceStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ResourceStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Indy.Sockets.Protocols.ResourceStrings", typeof(ResourceStrings).Assembly);
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
        ///   Looks up a localized string similar to This authentication method is already registered with class name {0}.
        /// </summary>
        internal static string AuthenticationMethodAlreadyRegistered {
            get {
                return ResourceManager.GetString("AuthenticationMethodAlreadyRegistered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unsupported hash algorithm. This implementation supports only MD5 encoding.
        /// </summary>
        internal static string InvalidHashMethod {
            get {
                return ResourceManager.GetString("InvalidHashMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Extension already exists.
        /// </summary>
        internal static string MIMEExtAlreadyExists {
            get {
                return ResourceManager.GetString("MIMEExtAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Extension is empty.
        /// </summary>
        internal static string MIMEExtensionEmpty {
            get {
                return ResourceManager.GetString("MIMEExtensionEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mimetype is empty.
        /// </summary>
        internal static string MIMEMIMETypeEmpty {
            get {
                return ResourceManager.GetString("MIMEMIMETypeEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Host field is empty.
        /// </summary>
        internal static string URINoHostSpecified {
            get {
                return ResourceManager.GetString("URINoHostSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Protocol field is empty.
        /// </summary>
        internal static string URINoProtocolSpecified {
            get {
                return ResourceManager.GetString("URINoProtocolSpecified", resourceCulture);
            }
        }
    }
}
