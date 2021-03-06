﻿//*********************************************************//
//    Copyright (c) Microsoft. All rights reserved.
//    
//    Apache 2.0 License
//    
//    You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//    
//    Unless required by applicable law or agreed to in writing, software 
//    distributed under the License is distributed on an "AS IS" BASIS, 
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or 
//    implied. See the License for the specific language governing 
//    permissions and limitations under the License.
//
//*********************************************************//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Microsoft.NodejsTools;
using Microsoft.VisualStudio.ComponentModelHost;

namespace Microsoft.NodejsTools.Profiling {
    /// <summary>
    /// Provides a view model for the StandaloneTarget class.
    /// </summary>
    public sealed class StandaloneTargetView : INotifyPropertyChanged {
        private string _interpreterPath;
        private string _workingDirectory;
        private string _scriptPath;
        private string _arguments;

        private bool _isValid;

        /// <summary>
        /// Create a StandaloneTargetView with default values.
        /// </summary>
        public StandaloneTargetView() {
            var componentService = (IComponentModel)(NodejsProfilingPackage.GetGlobalService(typeof(SComponentModel)));

            _interpreterPath = Nodejs.NodeExePath ?? String.Empty;
            _scriptPath = String.Empty;
            _workingDirectory = String.Empty;
            _arguments = null;

            _isValid = false;

            PropertyChanged += new PropertyChangedEventHandler(StandaloneTargetView_PropertyChanged);
        }

        /// <summary>
        /// Create a StandaloneTargetView with values taken from a template.
        /// </summary>
        public StandaloneTargetView(StandaloneTarget template)
            : this() {
            ScriptPath = template.Script;
            WorkingDirectory = template.WorkingDirectory ?? string.Empty;
            Arguments = template.Arguments;
        }

        /// <summary>
        /// Returns a StandaloneTarget with values taken from the view model.
        /// </summary>
        /// <returns></returns>
        public StandaloneTarget GetTarget() {
            if (IsValid) {
                return new StandaloneTarget {
                    InterpreterPath = InterpreterPath,
                    Script = ScriptPath ?? string.Empty,
                    WorkingDirectory = WorkingDirectory ?? string.Empty,
                    Arguments = Arguments ?? string.Empty
                };
            } else {
                return null;
            }
        }

        /// <summary>
        /// The current interpreter path. 
        /// </summary>
        public string InterpreterPath {
            get {
                return _interpreterPath;
            }
            set {
                if (_interpreterPath != value) {
                    _interpreterPath = value;
                    OnPropertyChanged("InterpreterPath");
                }
            }
        }

        /// <summary>
        /// The current script path.
        /// </summary>
        public string ScriptPath {
            get {
                return _scriptPath;
            }
            set {
                if (_scriptPath != value) {
                    _scriptPath = value;
                    OnPropertyChanged("ScriptPath");
                    //if (string.IsNullOrEmpty(WorkingDirectory)) {
                    //    WorkingDirectory = Path.GetDirectoryName(_scriptPath);
                    //}
                }
            }
        }

        /// <summary>
        /// The current working directory.
        /// </summary>
        public string WorkingDirectory {
            get {
                return _workingDirectory;
            }
            set {
                if (_workingDirectory != value) {
                    _workingDirectory = value;
                    OnPropertyChanged("WorkingDirectory");
                }
            }
        }

        /// <summary>
        /// The current set of arguments to pass to the script.
        /// </summary>
        public string Arguments {
            get {
                return _arguments;
            }
            set {
                if (_arguments != value) {
                    _arguments = value;
                    OnPropertyChanged("Arguments");
                }
            }
        }

        /// <summary>
        /// Receives our own property change events to update IsValid.
        /// </summary>
        void StandaloneTargetView_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            Debug.Assert(sender == this);

            if (e.PropertyName != "IsValid") {
                IsValid = ScriptPath.IndexOfAny(Path.GetInvalidPathChars()) == -1 &&
                    WorkingDirectory.IndexOfAny(Path.GetInvalidPathChars()) == -1 &&
                    (Path.IsPathRooted(ScriptPath) || Path.IsPathRooted(WorkingDirectory)) &&
                    File.Exists(Path.Combine(WorkingDirectory, ScriptPath)) &&
                    (WorkingDirectory == string.Empty || Directory.Exists(WorkingDirectory)) &&
                    (File.Exists(InterpreterPath));
            }
        }

        /// <summary>
        /// True if the settings are valid and all paths exist; otherwise, false.
        /// </summary>
        public bool IsValid {
            get {
                return _isValid;
            }
            private set {
                if (_isValid != value) {
                    _isValid = value;
                    OnPropertyChanged("IsValid");
                }
            }
        }

        private void OnPropertyChanged(string propertyName) {
            var evt = PropertyChanged;
            if (evt != null) {
                evt(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        /// <summary>
        /// Raised when the value of a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
 
