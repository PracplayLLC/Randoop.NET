using System;
using System.IO;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Text;
using VSLangProj;
using System.Collections.Generic;


namespace Randoop
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{

        private DTE2 _applicationObject;
        private AddIn _addInInstance;
        //private MenuManager _menuManager = null;
        //private ResourceManager _rm = new ResourceManager("Randoop.Randoop", Assembly.GetExecutingAssembly());

        //private CommandBarPopup _cmdBarPopup;
        //private CommandBarButton _btnReduce;
        //private CommandBarButton _btnMinimize;
        //private CommandBarButton _btnToMSTest;

        //string gInstallPath = "";
        //string gAppDataPath = "";
        //string gRandoop_path = "";

        //private StringBuilder processOutput = new StringBuilder(); //receive console output for Randoop reducer and minimizer
        

        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
            //getPaths();
		}

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;

			if(connectMode == ext_ConnectMode.ext_cm_UISetup)
            {
                object[] contextGUIDS = new object[] { };
                Commands2 commands = (Commands2)_applicationObject.Commands;
                string toolsMenuName;

                try
                {
                    //If you would like to move the command to a different menu, change the word "Tools" to the 
                    //  English version of the menu. This code will take the culture, append on the name of the menu
                    //  then add the command to that menu. You can find a list of all the top-level menus in the file
                    //  CommandBar.resx.
                    string resourceName;
                    ResourceManager resourceManager = new ResourceManager("Randoop.CommandBar", Assembly.GetExecutingAssembly());
                    CultureInfo cultureInfo = new CultureInfo(_applicationObject.LocaleID);

                    if (cultureInfo.TwoLetterISOLanguageName == "zh")
                    {
                        System.Globalization.CultureInfo parentCultureInfo = cultureInfo.Parent;
                        resourceName = String.Concat(parentCultureInfo.Name, "Tools");
                    }
                    else
                    {
                        resourceName = String.Concat(cultureInfo.TwoLetterISOLanguageName, "Tools");
                    }
                    toolsMenuName = resourceManager.GetString(resourceName);
                }
                catch
                {
                    //We tried to find a localized version of the word Tools, but one was not found.
                    //  Default to the en-US word, which may work for the current culture.
                    toolsMenuName = "Tools";
                }


                //Place the command on the tools menu.
                //Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
                Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];

                //Find the Tools command bar on the MenuBar command bar:
                CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
                CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

                //This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
                //  just make sure you also update the QueryStatus/Exec method to include the new command names.
                try
                {
                    //Add a command to the Commands collection:
                    //Command command = commands.AddNamedCommand2(_addInInstance, "Randoop", "Randoop...", "Executes the command for Randoop", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported+(int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                    Command command = commands.AddNamedCommand2(_addInInstance, "Randoop", "Randoop...", "Executes the command for Randoop", true, 1, ref contextGUIDS,
                        (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText,
                        vsCommandControlType.vsCommandControlTypeButton);

                    //Add a control for the command to the tools menu:
                    if ((command != null) && (toolsPopup != null))
                    {
                        command.AddControl(toolsPopup.CommandBar, 1);
                    }

                }
                catch (System.ArgumentException)
                {
                    //If we are here, then the exception is probably because a command with that name
                    //  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
                }


                //TODO: right click to activate (p2)
                //_menuManager = new MenuManager(_applicationObject);
                //Dictionary<string, CommandBase> cmds = new Dictionary<string, CommandBase>();
                //CommandBase randoopCommand = new RandoopCommand(_applicationObject);
                //cmds.Add("RandoopCommand", randoopCommand);
                //CommandBarPopup popupMenu = _menuManager.CreatePopupMenu("Assembly", _rm.GetString("RandoopMenuCaption"));
                //AddCommandMenu(popupMenu, cmds["RandoopCommand"], 1);

                #region post-randoop-functions
                //{
                //    //// Add a popup control to group our buttons under --- add submenu (new on Sep. 2012)

                //    _cmdBarPopup = (CommandBarPopup)toolsPopup.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, 2, true);
                //    _cmdBarPopup.Caption = "post Randoop";


                //    _btnReduce = (CommandBarButton)_cmdBarPopup.Controls.Add(MsoControlType.msoControlButton, Type.Missing,
                //        Type.Missing, Type.Missing, true);
                //    _btnReduce.Style = MsoButtonStyle.msoButtonIconAndCaption;
                //    _btnReduce.Caption = "Reducer";
                //    _btnReduce.TooltipText = "This command removes tests that it considers redundant";
                //    ////_btnFreebie.Picture = ImageConverter.ImageToIPicture(Resources.Resource.BarCode);
                //    _btnReduce.Click += new _CommandBarButtonEvents_ClickEventHandler(_btnReduce_Click);

                //    _btnMinimize = (CommandBarButton)_cmdBarPopup.Controls.Add(MsoControlType.msoControlButton, Type.Missing,
                //        Type.Missing, Type.Missing, true);
                //    _btnMinimize.Caption = "Minimizer";
                //    _btnMinimize.TooltipText = "This command transforms each test into a smaller one that still exhibits the same exception";
                //    //_btnLicensed.FaceId = 1845;
                //    _btnMinimize.Click += new _CommandBarButtonEvents_ClickEventHandler(_btnMinimize_Click);

                //    _btnToMSTest = (CommandBarButton)_cmdBarPopup.Controls.Add(MsoControlType.msoControlButton, Type.Missing,
                //        Type.Missing, Type.Missing, true);
                //    _btnToMSTest.Style = MsoButtonStyle.msoButtonIconAndCaption;
                //    _btnToMSTest.Caption = "Converter to MSTest";
                //    _btnToMSTest.Click += new _CommandBarButtonEvents_ClickEventHandler(_btnMSTest_Click);
                //}
                #endregion post-randoop-functions

            }            

		}


        #region post-randoop-implementations

        //public void _btnReduce_Click(CommandBarButton btn, ref bool cancel)
        //{
        //    string files = getSelectedFiles();

        //    if (files == "")
        //        return;

        //    using (System.Diagnostics.Process pRandoop = new System.Diagnostics.Process())
        //    {                
        //        pRandoop.StartInfo.FileName = gRandoop_path + "\\bin\\Randoop.exe";
        //        pRandoop.StartInfo.Arguments = "reduce \"" + files + "\"";

        //        pRandoop.StartInfo.CreateNoWindow = true;
        //        pRandoop.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; //run p in the background

        //        pRandoop.StartInfo.UseShellExecute = false;
        //        pRandoop.StartInfo.RedirectStandardOutput = true;
        //        //pRandoop.OutputDataReceived += reduce_OutputDataReceived;
        //        //processOutput.Length = 0; //clear string builder

        //        pRandoop.Start();

        //        while (!pRandoop.HasExited)
        //        {
        //            System.Threading.Thread.Sleep(10);
        //        }

        //        string output = pRandoop.StandardOutput.ReadToEnd();
        //        MessageBox.Show(output);
                
        //        //MessageBox.Show(processOutput.ToString());               
        //    }

        //}

        ////void reduce_OutputDataReceived(object sender, DataReceivedEventArgs e)
        ////{

        ////    string s = e.Data;

        ////    if (s != null)
        ////    {
        ////        processOutput.AppendLine(s);

        ////    }

        ////}


        //public void _btnMSTest_Click(CommandBarButton btn, ref bool cancel)
        //{
        //    string folder = getSelectedFolder(); 

        //    if (folder == "")
        //    {
        //        MessageBox.Show("Select a folder that contains test files");
        //        return;
        //    }

        //    toMSTest objToMsTest = new toMSTest();
        //    objToMsTest.Convert(folder, 1);

        //    //delete original randoop outputs except RandoopTest.cs, allstats.txt, etc.
        //    string[] files = Directory.GetFiles(folder);
        //    foreach (string file in files)
        //    {
        //        if (file.EndsWith("temp.cs") || file.EndsWith(".stats.txt") || file.EndsWith("index.html") || file.EndsWith("temp2.cs"))
        //            File.Delete(file);
        //    }
        //    File.Move(folder+"\\RandoopTest.cs", folder+"\\RandoopTest-reduced.cs");


        //    MessageBox.Show("Test cases in MSTest format are written in " + Environment.NewLine + folder + "\\RandoopTest-reduced.cs");

        //}


        //public void _btnMinimize_Click(CommandBarButton btn, ref bool cancel)
        //{
        //    string files = getSelectedFiles();

        //    if (files == "")
        //        return;

        //    using (System.Diagnostics.Process pRandoop = new System.Diagnostics.Process())
        //    {
        //        pRandoop.StartInfo.FileName = gRandoop_path + "\\bin\\Randoop.exe";
        //        pRandoop.StartInfo.Arguments = "minimize \"" + files + "\"";

        //        pRandoop.StartInfo.CreateNoWindow = true;
        //        pRandoop.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; //run p in the background

        //        pRandoop.Start();

        //        while (!pRandoop.HasExited)
        //        {
        //            System.Threading.Thread.Sleep(10);
        //        }

                
        //    }          

        //}

        //public void getPaths()
        //{
        //    gInstallPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        //    gAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        //    gRandoop_path = gAppDataPath + "\\Randoop-NET-release";
        //}


        //public string getSelectedFolder()
        //{
        //    MessageBox.Show("Select folder that contatins test files...");
        //    var dialog = new FolderBrowserDialog();
        //    string strPath = "";

        //    if (dialog.ShowDialog() == DialogResult.OK)
        //    {
        //        strPath = dialog.SelectedPath;
        //    }

        //    return strPath;
        //}
                
        
        //public string getSelectedFiles()
        //{
        //    MessageBox.Show("Select test files or folder that contains test cases to reduce or minimize...");
        //    var dialog = new OpenFileDialog();
        //    dialog.ValidateNames = false;
        //    dialog.CheckFileExists = false;
        //    dialog.CheckPathExists = true;
        //    dialog.FileName = "Folder Selection.";
        //    dialog.Multiselect = true;

        //    string arguments = "";

        //    //dialog.InitialDirectory = "";
        //    //dialog.DefaultExt = "cs";
                        
        //    if (dialog.ShowDialog() == DialogResult.OK)
        //    {
        //        string [] files = dialog.FileNames;
        //        int cnt = files.GetLength(0);

        //        if (cnt == 1)
        //        {
        //            int idx = files[0].LastIndexOf("\\");
        //            string filename = files[0].Substring(idx + 1);
        //            if (filename == "Folder Selection.")
        //                files[0] = files[0].Substring(0, idx);
        //        }                

        //        while (cnt > 0)
        //        {
        //            //MessageBox.Show(files[cnt - 1]);
        //            arguments += files[cnt - 1] + " ";
        //            cnt--;
        //        }
        //    }

        //    return arguments;
        //}
#endregion post-randoop-implementation


		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
            
		}

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom)
		{
		}
		
		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
				if(commandName == "Randoop.Connect.Randoop")
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
					return;
				}

                //if (commandName == "Randoop.Connect.Randoop2")
                //{
                //    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                //    return;
                //}
			}
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
		/// <param term='commandName'>The name of the command to execute.</param>
		/// <param term='executeOption'>Describes how the command should be run.</param>
		/// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param term='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
        /// 
        public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
        {
            handled = false;
            if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
            {
                if (commandName == "Randoop.Connect.Randoop")
                {
                    
                    randoopExe(_applicationObject);

                    handled = true;
                    return;
                }
            }
        }


        /// <summary>
        /// Adds the command menu.
        /// </summary>
        /// <param name="popupMenu">The popup menu.</param>
        /// <param name="command">The command.</param>
        /// <param name="position">The position.</param>
        //private void AddCommandMenu(CommandBarPopup popupMenu, CommandBase command, int position)
        //{
        //    if (popupMenu != null)
        //    {
        //        _menuManager.AddCommandMenu(popupMenu, command, position);
        //    }
        //}


        private void randoopExe(DTE2 application)
        {

            //MessageBox.Show("Enter randoopExe."); //debug

            
            //////////////////////////////////////////////////////////////////////////////////
            //step 1. when load randoop_net_addin, the path of "randoop" is defined 
            //////////////////////////////////////////////////////////////////////////////////
            string installPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);             
            var randoop_path = appDataPath + "\\Randoop-NET-release";

            //MessageBox.Show("root: " + randoop_path); //debug

            //////////////////////////////////////////////////////////////////////////////////
            // step 2. create win form (if an item is or is not selected in solution explorer)
            //////////////////////////////////////////////////////////////////////////////////

            UIHierarchy solutionExplorer = application.ToolWindows.SolutionExplorer;
            //MessageBox.Show("find solutionExplorer.");//debug

            var items = solutionExplorer.SelectedItems as Array;
            //MessageBox.Show("find selected items in solution.");//debug

            //try
            //{
                var arg = new Arguments(randoop_path);
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //}                     
                                
            if (items.Length == 1)
            {
                UIHierarchyItem item1 = items.GetValue(0) as UIHierarchyItem;
                var prJItem = item1.Object as ProjectItem;
                    
                if (prJItem != null)
                {
                    string prjPath = prJItem.Properties.Item("FullPath").Value.ToString();
                    if (prjPath.EndsWith(".dll") || prjPath.EndsWith(".exe"))
                        arg.SetDllToTest(prjPath);
                }
            }


            //////////////////////////////////////////////////////////////////////////////////
            // step 3. show the win form
            //////////////////////////////////////////////////////////////////////////////////
            //MessageBox.Show("before show dialog."); //debug
            arg.ShowDialog();

            if (arg.ifContinue() == false)
            {
                //MessageBox.Show("not going to execute Randoop."); 
                return;
            }


            //////////////////////////////////////////////////////////////////////////////////
            // step 4. prepare for Randoop.exe while reporting progress (invoked in background later)
            //////////////////////////////////////////////////////////////////////////////////

            string exepath = randoop_path + "\\bin\\Randoop.exe";

            if (!File.Exists(exepath))
            {
                MessageBox.Show("Can't find Randoop.exe!", "ERROR");
                return;
            }

            /***** load referenced .dlls  -- start ****/
            Solution curSolution = application.Solution;
            var prjs = curSolution.Projects;
            if (prjs == null)
            {
                MessageBox.Show("No project in current solution.", "ERROR");
                return;
            }

            List<String> referenceList = new List<string>();

            foreach (Project prj in prjs)
            {
                string prjFullName = "";
                string prjName = "";
                string prjPath = "";

                try
                {
                    prjFullName = prj.FullName;
                    prjName = System.IO.Path.GetFileName(prj.FullName);
                    prjPath = prjFullName.Remove(prjFullName.LastIndexOf(prjName));
                }
                catch
                {
                    continue;
                }               

                if ((prjPath == "")
                    || prj.Name.Contains("RandoopTestPrj") 
                    || !arg.GetDllToTest().Contains(prjPath)) //only consider reference in this project, NOT the whole solution
                {                        
                    continue;
                }

                VSProject prj2 = prj.Object as VSProject;  //get all references in the project
                if (prj2 == null)
                    continue;
                
                foreach (Reference reference in prj2.References)
                {
                    referenceList.Add(reference.Path);
                }

            }

            //copy the references to the place of .dll/exe to be test
            string dll_dir = arg.GetDllpath();
            List<string> addedrefList = new List<string>();
            try
            {
                foreach (String reffered_asm in referenceList)
                {
                    if (reffered_asm.Contains("Microsoft\\Framework"))                     
                        continue;                    

                    string fileName = System.IO.Path.GetFileName(reffered_asm);
                    string destFile = System.IO.Path.Combine(dll_dir, fileName);
                    File.Copy(reffered_asm, destFile, true); 
                    //TODO: referred .dll is already in place -- overwrite or not? -- should not and not add to the list to be delete
                    addedrefList.Add(destFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
                return;
            }
            /**** load referenced .dlls  -- end ****/

            var prg = new Progress();
            int totalTime = arg.GetTimeLimit();

            prg.getTotalTime(totalTime);
            prg.setRandoopExe(exepath);
            prg.setRandoopArg(arg.GetRandoopArg());


            //////////////////////////////////////////////////////////////////////////////////
            // step 5. prepare for convert all test files to one RandoopTest.cs (invoked in background later)
            //////////////////////////////////////////////////////////////////////////////////

            //MessageBox.Show("Randoop finishes generating test cases.", "Progress"); // [progress tracking]
            string out_dir = arg.GetTestFilepath();
            int nTestPfile = arg.GetTestNoPerFile();
            bool en_minimizer = arg.GetMinimizerStatus();
            int en_reducer = arg.GetReducerStatus();
            bool en_outplan = arg.GetOutputPlanStatus();

            prg.setOutDir(out_dir);
            prg.setTestpFile(nTestPfile);
            prg.setMinimizer(en_minimizer);
            prg.setReducer(en_reducer);
            prg.setOutPlan(en_outplan);
                           

            //invoke Randoop.exe and test file converting in the background by "progress" form
            prg.ShowDialog();

            if (prg.isNormal() == false)
            {
                //delete references copied to project bin
                try
                {
                    foreach (String reffered_asm in addedrefList)
                    {
                        File.Delete(reffered_asm);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("exception happens when deleting copied references:" + ex.Message, "EXCEPTION");                    
                }   

                return;
            }
                
            //////////////////////////////////////////////////////////////////////////////////
            // step 6. add/include RandoopTest.cs in a/the Test Project  
            //////////////////////////////////////////////////////////////////////////////////

            //MessageBox.Show("Creating a Test Project and add test files ...", "Progress"); // [progress tracking]
            string dllTest = arg.GetDllToTest();
            prg.setObjTested(dllTest);
            string pathToTestPrj = "";
            pathToTestPrj = CreateTestPrj(out_dir, dllTest, application);
            //MessageBox.Show("pathToTestPrj = " + pathToTestPrj); //debug
            if (pathToTestPrj != "failprj")
            {
                MessageBox.Show("Test file is created in project: " + pathToTestPrj);

                //invoke ie to open index.html generated by Randoop
                //System.Diagnostics.Process.Start("IEXPLORE.EXE", pathToTestPrj + "\\index.html"); //(p2) change it to an option later
            }
            else
            {
                MessageBox.Show("Test project creation fails." + Environment.NewLine + "But Test file can be found at: " + out_dir);
            }

            //MessageBox.Show("Task Completes!", "Progress"); // [progress tracking]
            

            //delete references copied to project bin
            try
            {
                foreach (String reffered_asm in addedrefList)
                {                    
                    File.Delete(reffered_asm); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "EXCEPTION");
                return;
            }

            return;
        }


        private string CreateTestPrj(string outDir, string dll_test, DTE2 application)
        {
            int step_tracking = 0;
            string str_prj_path = "";

            try
            {
                Solution curSolution = application.Solution;

                /////////////////////////////////////////////////////////////////////////////////////////////////
                //TODO: [step a] decide if there is a test project already existing in current solution (p4) 
                /////////////////////////////////////////////////////////////////////////////////////////////////
                
                //var prjs = curSolution.Projects;
                //if (prjs == null)
                //{
                //    MessageBox.Show(("no project object?"));
                //    return;
                //}

                //foreach (Project prj in prjs)
                //{
                //    //MessageBox.Show(prj.Kind); //debug
                //   //MessageBox.Show(prj.FullName); //debug
                //    var prjguid = new Guid(prj.Kind);
                //    if (IsVisualCSharpProject(prjguid))
                //        MessageBox.Show(prj.Name+" is C# project.", "type of project", MessageBoxButtons.OKCancel); //debug
                //    else 
                //    {
                //        if(IsTestProject(prjguid))
                //            MessageBox.Show(prj.Name + " is test project.", "type of project", MessageBoxButtons.OKCancel); //debug
                //        else
                //            MessageBox.Show(prj.Name + "is not a type that we care.", "type of project", MessageBoxButtons.OKCancel); //debug
                //    }
                //}
                         
                //////////////////////////////////////////////////////////////////////////////////////////////
                // [step b] automatically create a Test Project (particularly for Randoop generated tests)
                //////////////////////////////////////////////////////////////////////////////////////////////
                               
                int existRandoopTest = 0;
                var prjs = curSolution.Projects;
                if (prjs == null)
                {
                    MessageBox.Show("No project in current solution.","ERROR");
                    return null;
                }

                List<String> referenceList = new List<string>();

                foreach (Project prj in prjs)
                {
                    if (prj.Name.Contains("RandoopTestPrj"))
                    {
                        existRandoopTest = 1;
                        continue;
                    }

                    VSProject prj2 = prj.Object as VSProject;  //get all references in the project
                    if (prj2 == null)
                        continue;
                    foreach (Reference reference in prj2.References)
                    {
                       referenceList.Add(reference.Path);
                    }   
                  
                }

                //if "RandoopTestPrj" is not existing, create a new one
                if (existRandoopTest == 0)
                {
                    string testPrjPath = curSolution.FullName;
                    string prjName = "RandoopTestPrj";
                    int index = testPrjPath.LastIndexOf("\\");
                    testPrjPath = testPrjPath.Substring(0, index + 1) + prjName;
                    Solution2 soln = curSolution as Solution2;
                    string csTemplatePath = soln.GetProjectTemplate("TestProject.zip", "CSharp");
                    curSolution.AddFromTemplate(csTemplatePath, testPrjPath, prjName, false); //IMPORTANT: it always returns NULL
                }

                //locate the Randoop Test Project in current solution
                var allPrjs = curSolution.Projects;
                int idTestPrj = 1;
                foreach (Project prj in allPrjs)
                {
                    string prjFullName = "";
                    try
                    {
                        prjFullName = prj.FullName;
                    }
                    catch
                    {
                        idTestPrj++;
                        continue;
                    }

                    if (prj.FullName.Contains("RandoopTestPrj"))
                        break;
                    else
                        idTestPrj++;
                }

                ///////////////////////////////////////////////////////////////////////////////////////
                // [step c]  add/included converted RandoopTest.cs file under the project created above
                ///////////////////////////////////////////////////////////////////////////////////////

                string testFilePath = outDir + "\\RandoopTest.cs";
                string testHtmlPath = outDir + "\\index.html";
                string testStatPath = outDir + "\\allstats.txt";
                bool isAdded1 = false;
                bool isAdded2 = false;
                bool isAdded3 = false;

                //TODO: (t_cov) used for OpenCover (opt.)
                //string testFile2Path = outDir + "\\RandoopTestGeneral.cs"; 
                //string testdriverPath = outDir + "\\TestDriver.cs"; 
                //bool isAdded4 = false; 
                //bool isAdded5 = false; 

                Project testPrj = curSolution.Projects.Item(idTestPrj);
                if (testPrj != null)
                {   
                    foreach (ProjectItem it in testPrj.ProjectItems)
                    {
                        if (it.Name.Equals("RandoopTest.cs") && (isAdded1 == false))
                        {
                            it.Delete();
                            testPrj.ProjectItems.AddFromFileCopy(testFilePath);
                            isAdded1 = true;
                            continue;
                        }
                        if (it.Name.Equals("index.html") && (isAdded2 == false))
                        {
                             it.Delete();
                             testPrj.ProjectItems.AddFromFileCopy(testHtmlPath);
                             isAdded2 = true;
                             continue;
                         }
                        if (it.Name.Equals("allstats.txt") && (isAdded3 == false))
                         {
                                it.Delete();
                                testPrj.ProjectItems.AddFromFileCopy(testStatPath);
                                isAdded3 = true;
                                continue;
                          }
                        ////TODO: (t_cov) used for OpenCover [start]
                         //if (it.Name.Contains("RandoopTestGeneral.cs") && (isAdded4 == false))
                         //{
                         //    it.Delete();
                         //    testPrj.ProjectItems.AddFromFileCopy(testFile2Path);
                         //    isAdded4 = true;
                         //    continue;

                         //}
                         //if (it.Name.Contains("TestDriver.cs") && (isAdded5 == false))
                         //{
                         //    it.Delete();
                         //    testPrj.ProjectItems.AddFromFileCopy(testdriverPath);
                         //    isAdded5 = true;
                         //    continue;
                         //}
                        ////used for OpenCover [end]
                    }

                    if (! isAdded1)
                        testPrj.ProjectItems.AddFromFileCopy(testFilePath);

                    if (! isAdded2)
                        testPrj.ProjectItems.AddFromFileCopy(testHtmlPath);

                    if (! isAdded3)
                        testPrj.ProjectItems.AddFromFileCopy(testStatPath);
                    ////TODO: (t_cov) used for OpenCover [start]
                    //if(! isAdded4)
                    //    testPrj.ProjectItems.AddFromFileCopy(testFile2Path);

                    //if(! isAdded5)
                    //    testPrj.ProjectItems.AddFromFileCopy(testdriverPath);
                    ////used for OpenCover [end]


                    //if (testPrj.ProjectItems.Count > 2) //default true
                    {
                        foreach (ProjectItem it in testPrj.ProjectItems)
                        {
                            if (it.Name.Contains("UnitTest1.cs")) 
                                it.Delete();
                        }
                    }


                    step_tracking = 1; //mark that "the test project was successfully created"
                    str_prj_path = testPrj.FullName.Replace("\\RandoopTestPrj.csproj", "");


                    //delete original randoop outputs except RandoopTest.cs, allstats.txt, etc.
                    string[] files = Directory.GetFiles(outDir);
                    foreach (string file in files)
                    {
                        if (file.EndsWith("temp.cs") || file.EndsWith(".stats.txt") || file.EndsWith("index.html") || file.EndsWith("temp2.cs"))
                            File.Delete(file);
                    }
                    //string[] subdirs = Directory.GetDirectories(outDir);
                    //foreach (string subdir in subdirs)
                    //    Directory.Delete(subdir, true);

                    step_tracking = 2; //mark that "deletion completed"
                                        

                    //Programmatically add references to project under test 
                    VSProject selectedVSProject = null;
                    selectedVSProject = (VSProject)testPrj.Object;
                    selectedVSProject.References.Add(dll_test);

                    foreach (string reference in referenceList)
                    {
                        int _num_ref = selectedVSProject.References.Count;
                        bool _ref_exists = false;

                        for (int i = 1; i <= _num_ref; i++)
                        {
                            //if (selectedVSProject.References.Item(i).Path == reference) //same .dll, from different paths
                            if (selectedVSProject.References.Item(i).Path.Substring(selectedVSProject.References.Item(i).Path.LastIndexOf("\\")+1) 
                                ==  reference.Substring(reference.LastIndexOf("\\")+1))
                            {
                                _ref_exists = true;
                                break;
                            }
                        }

                        if (!_ref_exists) //avoid duplicated reference (VS2010 does not allow adding duplicated references)
                        {
                            selectedVSProject.References.Add(reference);
                        }
                    }                    

                    return (str_prj_path);

                }                

            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "EXCEPTION");
                if (step_tracking < 2)
                    System.Windows.Forms.MessageBox.Show("NOTE: May remove the \"unavailable\" Randoop test project in the Solution Explorer.", "EXCEPTION");
                if(step_tracking == 2)
                    System.Windows.Forms.MessageBox.Show("NOTE: May need manually adding references to the test project.", "EXCEPTION");
            }

            if (step_tracking == 0)
                return "failprj";
            else
            {
                //since the test project and file is successfully created, we can safely delete "backup" test files
                Directory.Delete(outDir, true);

                return (str_prj_path);                
            }

        }


        public static bool IsVisualCSharpProject(Guid projectKind)
        {
            return projectKind.CompareTo(new Guid("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}")) == 0;
        }

        public static bool IsTestProject(Guid projectKind)
        {
            return projectKind.CompareTo(new Guid("{3AC096D0-A1C2-E12C-1390-A8335801FDAB}")) == 0;
        }
        

    }
         
         
}