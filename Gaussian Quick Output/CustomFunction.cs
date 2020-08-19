using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace Gaussian_Quick_Output
{
   
    [XmlRoot]
    public class CustomFunctions : INotifyPropertyChanged
    {
        public CustomFunctions()
        {
            FunctionList = new List<CustomFunction>();
        }
        private List<CustomFunction> _functionList = new List<CustomFunction>();

        private List<StringOccurenceFunction> _stringOccurenceFunctionList = new List<StringOccurenceFunction>();
        private List<AbsoluteSearchFunction> _absoluteSearchFunctionList = new List<AbsoluteSearchFunction>();
   
        public List<CustomFunction> FunctionList
        {
            get { return _functionList; }
            set
            {
                _functionList = value;
                OnPropertyChanged(this, "FunctionList");
                
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        private void OnPropertyChanged(object sender, string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
        }

        public string SerializeCollection()
        {
            return JsonConvert.SerializeObject(FunctionList, Formatting.Indented);
        }
    }
    
    public abstract class CustomFunction
    {
        
        public string Name { get; set; }
        public virtual string ReadFunction(string file)
        {
            return "";
        }
        public virtual void DoFunction(string filename)
        {

        }

        public static CustomFunction Build(System.Reflection.MethodInfo method)
        {
            int yPos = 0;
            Form7 form = new Form7();
            List<Control> paramControlList = new List<Control>();
            if (method.GetParameters().Length != 0)
            {
                for (int i = 0; i < method.GetParameters().Length; i++)
                {

                    if (method.GetParameters()[i].ParameterType == typeof(string))
                    {
                        TextBox t = new TextBox();
                        t.Tag = typeof(string);
                        Label lt = new Label();
                        lt.Text = method.GetParameters()[i].Name;
                        t.Location = new System.Drawing.Point(5, yPos);
                        lt.Location = new System.Drawing.Point(200, yPos);
                        form.Controls.Add(t);
                        form.Controls.Add(lt);
                        t.Show();
                        lt.Show();
                        paramControlList.Add(t);


                    }
                    if (method.GetParameters()[i].ParameterType == typeof(int))
                    {
                        TextBox t = new TextBox();
                        t.Tag = typeof(int);
                        t.KeyPress += new KeyPressEventHandler(sanitizeInput);
                        Label lt = new Label();
                        lt.Text = method.GetParameters()[i].Name;
                        t.Location = new System.Drawing.Point(5, yPos);
                        lt.Location = new System.Drawing.Point(200, yPos);
                        form.Controls.Add(t);
                        form.Controls.Add(lt);
                        t.Show();
                        lt.Show();
                        paramControlList.Add(t);
                    }
                    yPos += 80;
                }
                Button submit = new Button();
                form.Controls.Add(submit);
                submit.DialogResult = DialogResult.OK;
                submit.Location = new System.Drawing.Point(90, yPos + 50);
                submit.Text = "Submit";
                submit.Show();
                form.AutoSize = true;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    List<object> parameters = new List<object>();
                    foreach (Control c in paramControlList)
                    {
                        if ((Type)c.Tag == typeof(int))
                        {
                            if (int.TryParse(c.Text, out int x))
                            {
                                parameters.Add(x);
                            }
                        }
                        else
                        {
                            parameters.Add(c.Text);
                        }
                    }
                    MessageBox.Show("dialog successful");
                    return (CustomFunction)(method.Invoke(method, parameters.ToArray()));

                }
                else return null;
            }
            else return null;
        }
        private static void sanitizeInput(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)Keys.Back) //The  character represents a backspace
            {
                e.Handled = false; //Do not reject the input
            }
            else
            {
                e.Handled = true; //Reject the input
            }
        }
    }
    
    public class StringOccurenceFunction :CustomFunction
    {
        private StringOccurenceFunction()
        {

        }
        public string SearchKeyword { get; set; }
        public StringOccurenceFunction(string name, string searchKeyword)
        {
            Name = name;
            SearchKeyword = searchKeyword;
        }

        public static CustomFunction Create(string name, string searchKeyword)
        {
            CustomFunction c = new StringOccurenceFunction(name, searchKeyword);

            return c;
            
        }
       

        override public string ReadFunction(string file)
        {
           return CountStringOccurrences(file, SearchKeyword).ToString();
        }
        private static int CountStringOccurrences(string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }
    }

    public class AbsoluteSearchFunction : CustomFunction
    {
        private AbsoluteSearchFunction()
        {

        }
        public string SearchTerm { get; set; }
        public int CharsAfter { get; set; }
        public int OutputChars { get; set; }

        public AbsoluteSearchFunction(string name, string searchTerm, int charsAfter, int outputChars)
        {
            Name = name;
            SearchTerm = searchTerm;
            CharsAfter = charsAfter;
            OutputChars = outputChars;
        }
        public static CustomFunction Create(string name, string searchTerm, int charsAfter, int outputChars)
        {
            CustomFunction c = new AbsoluteSearchFunction(name, searchTerm, charsAfter, outputChars);
            
            
            return c;
        }
        private void sanitizeInput(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)Keys.Back) //The  character represents a backspace
            {
                e.Handled = false; //Do not reject the input
            }
            else
            {
                e.Handled = true; //Reject the input
            }
        }
        public override string ReadFunction(string file)
        {
            try
            {
                if (file.IndexOf(SearchTerm) == -1)
                {
                    return "N/A";
                }

                else
                {
                    return file.Substring(file.IndexOf(SearchTerm) + CharsAfter, OutputChars);
                }
            }
            catch (Exception e)
            {
                return "N/A";
            }
            
          
        }
    }
    public class FindAndReplaceFunction : CustomFunction
    {
        private FindAndReplaceFunction()
        {

        }
        public string Find { get; set; }
        public string Replace { get; set; }

        public FindAndReplaceFunction(string name, string find, string replace)
        {
            Name = name;
            Find = find;
            Replace = replace;
        }
        public static FindAndReplaceFunction Create (string name, string find, string replace)
        {
            FindAndReplaceFunction fr = new FindAndReplaceFunction(name, find, replace);
            return fr;
        }
        public override string ReadFunction(string file)
        {
            StringOccurenceFunction c = new StringOccurenceFunction("temp", Find);
            return "Found " + c.ReadFunction(file)+  " items to replace";
        }
        public override void DoFunction(string filename)
        {
            string filetext = System.IO.File.ReadAllText(filename);
            filetext = filetext.Replace(Find, Replace);
            try
            {
                System.IO.File.WriteAllText(filename, filetext);
            }
            catch (Exception)
            {

            }
        }

    }
}



