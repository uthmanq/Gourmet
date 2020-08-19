using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Gaussian_Quick_Output
{
    public partial class Form6 : Form
    {
        private bool NewDocument;
        string fileContent = string.Empty;
        string filePath = string.Empty;
        public CustomFunctions SessionTemplate = new CustomFunctions();
        BindingSource bindingSource = new BindingSource();

        public Form6(bool newDocument)
        {
            InitializeComponent();
            NewDocument = newDocument;

        }

        private void Form6_Load(object sender, EventArgs e)
        {
            if (!NewDocument)
            {
                LoadTemplate();
            }
            UpdateTemplate();
        }
        public void LoadTemplate()
        {

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                filePath = openFileDialog1.FileName;

                //Read the contents of the file into a stream
                var fileStream = openFileDialog1.OpenFile();
                XmlSerializer ser = new XmlSerializer(typeof(CustomFunctions), new Type[] { typeof(AbsoluteSearchFunction), typeof(StringOccurenceFunction) });

                StreamReader rdr = new StreamReader(filePath);

                SessionTemplate = (CustomFunctions)ser.Deserialize(rdr);


            }
        }
        public void UpdateTemplate()
        {
            listBox1.Items.Clear();
            if (SessionTemplate.FunctionList.Count > 0)
            {
                foreach (CustomFunction c in SessionTemplate.FunctionList)
                {
                    if (c != null)
                    {
                        listBox1.Items.Add(c);
                    }
                    listBox1.DisplayMember = "name";
                }
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                XmlSerializer ser = new XmlSerializer(typeof(CustomFunctions), new Type[] { typeof(AbsoluteSearchFunction), typeof(StringOccurenceFunction), typeof(FindAndReplaceFunction) });
                using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                {
                    ser.Serialize(fs, SessionTemplate);
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //makeList();
            //UpdateTemplate();
            //string name = "efdsgsfd";
            //   string searchTerm = "efdsgsfd"; int charsAfter = 6; int outputChars = 3;
            // AbsoluteSearchFunction f = new AbsoluteSearchFunction(name, searchTerm, charsAfter, outputChars);
            //  f.Build();
            if (comboBox1.Text == "Search and return a related value")
            {
                System.Reflection.MethodInfo meth = typeof(AbsoluteSearchFunction).GetMethod("Create");
                SessionTemplate.FunctionList.Add(CustomFunction.Build(meth));
                UpdateTemplate();
            }
            if (comboBox1.Text == "Count number of times a search term occurs")
            {
                System.Reflection.MethodInfo meth = typeof(StringOccurenceFunction).GetMethod("Create");
                SessionTemplate.FunctionList.Add(CustomFunction.Build(meth));
                UpdateTemplate();
            }
            if (comboBox1.Text == "Find and Replace")
            {
                System.Reflection.MethodInfo meth = typeof(FindAndReplaceFunction).GetMethod("Create");
                SessionTemplate.FunctionList.Add(CustomFunction.Build(meth));
                UpdateTemplate();
            }
            //listBox1.SelectedIndex = 0;
            // MessageBox.Show(listBox1.SelectedItem.GetType().ToString());

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            if (listBox1.SelectedIndex != -1)
            {
                for (int i = 0; i < listBox1.SelectedItem.GetType().GetMethod("Create").GetParameters().Length; i++)
                {
                //    comboBox1.Items.Add(listBox1.SelectedItem.GetType().GetMethod("Create").GetParameters()[i].Name);
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                SessionTemplate.FunctionList.Remove((CustomFunction)listBox1.SelectedItem);
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
        }
    }
}
