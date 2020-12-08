using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
//for dynamo
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Threading;
//for open dialog
using Microsoft.Win32;

namespace MathPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Dynamo : Window
    {
        readonly static string sLogFile = "mathpanel.log";
        readonly static string version = " v1.0";
        static bool bReady = false;
        static System.Windows.Threading.Dispatcher dispObj; //dispatcher of UI-thread, we will access UI elements through it
        static TextBox txtConsole, txtCommand;
        static WebBrowser webConsole;
        static Image imgStatic;

        static string frmPath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\";
        static string keyConsole = "";
        static object dynClassInstance = null;
        static System.Threading.Thread my_thread = null;
        readonly static List<System.Threading.Thread> lstThr = new List<Thread>();

        public Dynamo()
        {
            InitializeComponent();
            //hide 2 controls
            web1.Visibility = Visibility.Hidden;
            img1.Visibility = Visibility.Hidden;

            this.Title += version;
            button1.Click += Button1_Click;
            button2.Click += Button2_Click;
            button3.Click += Button3_Click;
            button4.Click += Button4_Click;
            button5.Click += Button5_Click;
            button6.Click += Button6_Click;
            button7.Click += Button7_Click;

            dispObj = Dispatcher;
            txtCommand = textBlock1;
            txtConsole = textBlock2;
            webConsole = web1;
            imgStatic = img1;
            this.Closing += OnBeforeClosing;
            bReady = true;

            string sDir = AppDomain.CurrentDomain.BaseDirectory;
            web1.Navigate("file:///" + sDir + "test_graph.htm");

        }
        //handler for button "Execute"
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (!bReady)
            {
                MessageBox.Show("Not ready");
                return;
            }
            Process(textBlock1.Text);
        }
        //hide/show controls
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            if (img1.IsVisible)
            {
                img1.Visibility = Visibility.Hidden;
                Label1.Visibility = Visibility.Hidden;
                Label2.Visibility = Visibility.Hidden;
                sv1.Visibility = Visibility.Hidden;
                textBlock2.Visibility = Visibility.Hidden;
                web1.Visibility = Visibility.Visible;
            }

            Label1.Visibility = Label1.IsVisible ? Visibility.Hidden : Visibility.Visible;
            Label2.Visibility = Label2.IsVisible ? Visibility.Hidden : Visibility.Visible;
            sv1.Visibility = sv1.IsVisible ? Visibility.Hidden : Visibility.Visible;
            textBlock2.Visibility = textBlock2.IsVisible ? Visibility.Hidden : Visibility.Visible;
            web1.Visibility = web1.IsVisible ? Visibility.Hidden : Visibility.Visible;
            button2.Content = Label1.IsVisible ? "Graphics" : "Commands";
        }
        //load script
        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            string data;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                data = File.ReadAllText(openFileDialog.FileName, Encoding.UTF8);
                textBlock1.Text = data;
            }
        }
        //save script
        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                CheckFileExists = false
            };
            if (openFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(openFileDialog.FileName, textBlock1.Text, Encoding.UTF8);
            }
        }
        //handler for "Compile"
        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                CheckFileExists = false
            };
            if (openFileDialog.ShowDialog() == false) return;
            string fname = openFileDialog.FileName;
            if (fname.LastIndexOf(".cs") == fname.Length - 3)
            {
                var data = File.ReadAllText(openFileDialog.FileName, Encoding.UTF8);
                textBlock1.Text = data;
                fname = fname.Substring(0, fname.Length - 3) + ".dll";
            }
            string sourceDll = textBlock1.Text;
            string outputAssembly = fname;
            CompilerResults results;
            try
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                //build dll
                CompilerParameters compilerParams2 = new CompilerParameters
                { OutputAssembly = outputAssembly, GenerateExecutable = false };
                //to allow LINQ
                compilerParams2.ReferencedAssemblies.Add("System.Core.Dll");

                // compile dll
                results = provider.CompileAssemblyFromSource(compilerParams2, sourceDll);
                // output errors
                Console(string.Format("Number of Errors DLL: {0}", results.Errors.Count));
                foreach (CompilerError err in results.Errors)
                {
                    Console(string.Format("ERROR {0}", err.ErrorText));
                }
                /*//test
                Console(string.Format("Try Assembly:"));
                Assembly assembly = Assembly.LoadFile(outputAssembly);
                Type type = assembly.GetType("MathPanelExt.QuadroEqu");
                MethodInfo method = type.GetMethod("Solve");
                double x1 = 0, x2 = 0;
                object[] pars = new object[5];
                pars[0] = 1.0;
                pars[1] = 0.0;
                pars[2] = -1.0;
                pars[3] = null;
                pars[4] = null;
                object res = method.Invoke(null, pars);
                //bool blResult = (bool)res;
                //if (blResult)
                {
                    x1 = (double)pars[3];
                    x2 = (double)pars[4];
                }
                Console(string.Format("x1 {0}, x2 {1}", x1, x2));
                //end of test*/
            }
            catch (Exception ex) { Console(ex.ToString()); }
            return;
        }
        //new script
        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            string data = File.ReadAllText("template.cs", Encoding.UTF8);
            textBlock1.Text = data;
        }
        //load image
        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            Label1.Visibility = Visibility.Hidden;
            Label2.Visibility = Visibility.Hidden;
            sv1.Visibility = Visibility.Hidden;
            textBlock2.Visibility = Visibility.Hidden;
            web1.Visibility = Visibility.Hidden;
            img1.Visibility = Visibility.Visible;
            button2.Content = "Commands";

            string path;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileName;
                var bitmap = new System.Windows.Media.Imaging.BitmapImage(new Uri(path, UriKind.Absolute));
                img1.Source = bitmap;
            }
        }

        //called immediately after Window.Close() and can be used to cancel command
        void OnBeforeClosing(object sender, EventArgs e)
        {
            bReady = false;
            foreach (var thr in lstThr)
            {
                if (thr.IsAlive) thr.Abort();
            }
        }


        /// <summary>
        /// write to log-file
        /// </summary>
        public static void Log(string s)
        {
            try
            {
                StreamWriter sw = new StreamWriter(sLogFile, true, Encoding.UTF8);
                sw.WriteLine(string.Format("{0:yyyy-MM-dd HH:mm:ss} {1}", DateTime.Now, s));
                sw.Close();
            }
            catch (Exception se)
            {
                //Console(se.Message);
                MessageBox.Show(se.ToString());
            }
        }
        /// <summary>
        /// add text to console window
        /// </summary>
        /// <param name="s">text</param>
        public static void Console(string s, bool bNewLine = true)
        {
            //txtConsole.Text += (s + "\r\n");
            if (!bReady) return;
            //launch the code in UI thread
            dispObj.Invoke(delegate
            {
                if (bReady) txtConsole.Text += (s + (bNewLine ? "\r\n" : ""));
            });
        }

        //helper for Process
        static object CompileDynamo(string code, Type outType = null, string[] includeNamespaces = null, string[] includeAssemblies = null)
        {
            StringBuilder namespaces = null;
            object methodResult = null;
            dynClassInstance = null;
            using (CSharpCodeProvider codeProvider = new CSharpCodeProvider())
            {
                CompilerParameters compileParams = new CompilerParameters
                {
                    CompilerOptions = "/t:library",
                    GenerateInMemory = true
                };

                int ipos = code.IndexOf("///[DLL]");
                if (ipos > 0)
                {
                    int ipos2 = code.IndexOf("[/DLL]", ipos);
                    if (ipos2 > 0)
                    {
                        compileParams.ReferencedAssemblies.Add("MathPanel.exe");
                        string ass = code.Substring(ipos + 8, ipos2 - ipos - 8);
                        var arr = ass.Split(',');
                        foreach (string _assembly in arr)
                        {
                            compileParams.ReferencedAssemblies.Add(frmPath + _assembly.Trim());
                        }
                    }
                }
                else if (includeAssemblies != null && includeAssemblies.Length > 0)
                {
                    foreach (string _assembly in includeAssemblies)
                    {
                        compileParams.ReferencedAssemblies.Add(_assembly);
                    }
                }

                if (includeNamespaces != null && includeNamespaces.Length > 0)
                {
                    namespaces = new StringBuilder();
                    foreach (string _namespace in includeNamespaces)
                    {
                        namespaces.Append(string.Format("using {0};\n", _namespace));
                    }
                }

                if (code.IndexOf("namespace DynamoCode") < 0)
                    code = string.Format(
                        @"{1}  
                using System;  
                namespace DynamoCode{{  
                    public class Script{{  
                        public {2} Execute(){{  
                            {3} {0};  
                        }}  
                    }}  
                }}",
                        code,
                        namespaces != null ? namespaces.ToString() : null,
                        outType != null ? outType.FullName : "void",
                        outType != null ? "return" : string.Empty
                        );
                CompilerResults compileResult = codeProvider.CompileAssemblyFromSource(compileParams, code);

                if (compileResult.Errors.Count > 0)
                {
                    Console("compile error: " + compileResult.Errors[0].ErrorText);
                    return "compile error: " + compileResult.Errors[0].ErrorText;
                }
                System.Reflection.Assembly assembly = compileResult.CompiledAssembly;
                dynClassInstance = assembly.CreateInstance("DynamoCode.Script");
            }
            return methodResult;
        }


        //compile and execute commands
        public static void Process(string s)
        {
            string[] includeAssemblies = { "MathPanel.exe",
                frmPath + "System.dll",
                frmPath + "System.Xaml.dll",
                frmPath + "WindowsBase.dll",
                frmPath + "PresentationFramework.dll",
                frmPath + "PresentationCore.dll"
                , frmPath + "System.Drawing.dll"
                , frmPath + "System.Net.dll"
                , frmPath + "System.Net.Http.dll"
            };
            string[] includeNamespaces = { "MathPanel", "MathPanelExt", "System.Net.Sockets" };
            keyConsole = "";
            CompileDynamo(s, null, includeNamespaces, includeAssemblies);
            //eval in a new thread
            try
            {
                if (dynClassInstance != null)
                {
                    my_thread = new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                        Type type = dynClassInstance.GetType();
                        MethodInfo methodInfo = type.GetMethod("Execute");
                        try
                        {
                            methodInfo.Invoke(dynClassInstance, null);
                            Dynamo.Console("Done");
                        }
                        catch (Exception yyy) { Dynamo.Console(yyy.ToString()); }
                    }));
                    my_thread.Start();
                    lstThr.Add(my_thread);
                }
            }
            catch (Exception xxx) { Dynamo.Console(xxx.ToString()); }
        }

        //2020-11-13

        //load image file into control
        public static void SetBitmapImage(string path)
        {
            if (!bReady) return;
            //launch in UI thread
            dispObj.Invoke(delegate
            {
                imgStatic.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(path, UriKind.Absolute));
            });
        }

        /// <summary>
        /// set html into control
        /// </summary>
        /// <param name="data">html string</param>
        public static void SetHtml(string data)
        {
            if (!bReady || dispObj.HasShutdownStarted) return;
            //launch in UI thread
            dispObj.Invoke(delegate
            {
                webConsole.InvokeScript("ext_div", data);
            });
        }

        //2020-11-15
        public static void GraphExample(string id)
        {
            if (!bReady || dispObj.HasShutdownStarted) return;
            //launch in UI thread
            dispObj.Invoke(delegate
            {
                webConsole.InvokeScript("ext_example" + id);
            });
        }
        static string screenJson = "";
        /// <summary>
        /// send JSON data for visualization in canvas
        /// </summary>
        public static void SceneJson(string s_json, bool bSecond = false)
        {
            if (!bReady || dispObj.HasShutdownStarted) return;
            screenJson = s_json;
            //launch in UI thread
            dispObj.Invoke(delegate
            {
                webConsole.InvokeScript("ext_json", screenJson, bSecond);
            });
        }

        public static string D2S(double d)
        {   //F4
            string s = d.ToString("G4", System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            if (s.Contains("NaN"))
            {   //to catch exceptions
                int kk = 0;
            }
            return s;
        }

        //2020-12-08
        private void MyPreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift && e.Key == Key.F1)
            string key = e.Key.ToString();
            //Console(key);
            keyConsole = key;
        }

        /// <summary>
        /// returns last pressed key
        /// </summary>
        /// <return>last key</return>
        public static string KeyConsole
        {
            get
            {
                var s = keyConsole;
                keyConsole = "";
                return s;
            }
        }

    }
}
