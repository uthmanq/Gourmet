using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;

namespace Gaussian_Quick_Output
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            customFunctions = new CustomFunctions();
            bindingSource = new BindingSource();
            bindingSource.DataSource = customFunctions.FunctionList;
            //customFunctions.FunctionList.Add(AbsoluteSearchFunction.Create("Enthalpies", "Enthalpies", 43, 431));
            //customFunctions.FunctionList.Add(StringOccurenceFunction.Create("Item", "Item"));
            //comboBox1.DataSource = bindingSource.DataSource;
           // comboBox1.DisplayMember = "Name";
           // comboBox1.ValueMember = "Name";
            //checkedListBox1.DataSource = bindingSource.DataSource;
            //checkedListBox1.DisplayMember = "Name";
            //checkedListBox1.ValueMember = "Name";


        }
        public CustomFunctions customFunctions;
        BindingSource bindingSource;

        public static string dataset = "";
        public static DataTable valueTable = new DataTable();
        public static string fullreport = "";
        public static List<string> fileList = new List<string>();

        private void button1_Click(object sender, EventArgs e)
        {
            ChooseFolder();
        }

        //Function to preview data of log files
        //Some data validation would be nice, but not completely necessary
        public void dataLookup()
        {
            if (listBox1.SelectedIndex != -1)
            {
                string file = System.IO.File.ReadAllText(listBox1.Text);
                CustomFunction c = (CustomFunction)comboBox1.SelectedItem;
                textBox1.Text = c.ReadFunction(file);
            }

        }

        public void processData()
        {


        }
        public string processReport()
        {
            if (!String.IsNullOrEmpty(folderBrowserDialog1.SelectedPath))
            {
                string genereatedReport = "Generated Report: \n \n";
                int i = 0;
                foreach (string file in Directory.GetFiles(folderBrowserDialog1.SelectedPath))
                {
                    if (file.EndsWith(Properties.Settings.Default.fileTypeSearch))
                    {
                        i++;
                        string filetext = System.IO.File.ReadAllText(file);
                        string filename = file.Substring(file.LastIndexOf(@"\") + 1);
                        string report = Report(filetext);

                        genereatedReport += string.Format("Report for: {0} \n \n {1} \n \n", filename, report);
                    }
                }
                MessageBox.Show(string.Format("Quick Output found {0} log files to analyze. Saving now will create a report sheet with {0} entries. Please review the data once it is completed. This is a BETA function and results may be incomplete or inconclusive", i.ToString()), "Report Analysis Status");
                fullreport = genereatedReport;
                return genereatedReport;
            }
            else
            {
                return "";
            }
        }
        public static string Report(string text)
        {
            string part1 = text.Substring(text.LastIndexOf("SCF Done:"), 74);
            string part2helper = text.Substring(text.LastIndexOf("Zero-point correction="), 1364);
            string part2 = part2helper.Substring(0, part2helper.LastIndexOf("Electronic"));
            string part3helper = text.Substring(text.LastIndexOf("Center     Atomic                   Forces (Hartrees/Bohr)"));
            string part3 = part3helper.Substring(0, part3helper.IndexOf("Cartesian"));
            string total = part1 + "\n" + part2 + "\n" + part3;
            return total;

        }
        public void ChooseFolder()
        {
            fileList.Clear();
            //A little bit of QoL code to set the default file to be the most recent file selected. 
            //Reduces the headache of navigating through hella directories just to find your folder
            //Just a little code snippet from Stackoverflow
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!String.IsNullOrEmpty(Properties.Settings.Default.lastDirectory))
                {
                    Properties.Settings.Default.lastDirectory = folderBrowserDialog1.SelectedPath;
                }
                Properties.Settings.Default.lastDirectory = folderBrowserDialog1.SelectedPath;
                Properties.Settings.Default.Save();

                //Encapsulated in a try/catch block so program doesnt die because of any exception
                //Simply populates the list with all log files
                listBox1.Items.Clear();
                try
                {
                    folderBrowserDialog1.SelectedPath = Properties.Settings.Default.lastDirectory;
                    foreach (string file in Directory.GetFiles(folderBrowserDialog1.SelectedPath))
                    {
                        //Not sure if there's a smarter way to do this but this seems pretty fool-proof
                        if (file.EndsWith(Properties.Settings.Default.fileTypeSearch))
                        {
                            listBox1.Items.Add(file);
                            fileList.Add(file);
                        }
                    }
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString());
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataLookup();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
        }

        //Method saves everything by processing the data and opening up file system dialog. 
        //Can be expanded upon in the future in case further customization is needed, but right now this is perfectly fine
        private void button2_Click(object sender, EventArgs e)
        {
            processData();

            var lines = new List<string>();
            string[] columnNames = valueTable.Columns
                .Cast<DataColumn>()
                .Select(column => column.ColumnName)
                .ToArray();

            var header = string.Join(",", columnNames.Select(name => $"\"{name}\""));
            lines.Add(header);

            var valueLines = valueTable.AsEnumerable()
           .Select(row => string.Join(",", row.ItemArray.Select(val => $"\"{val}\"")));

            lines.AddRange(valueLines);


            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(saveFileDialog1.FileName, lines);

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show(Properties.Settings.Default.currentXMLTemplate);
            if (!String.IsNullOrEmpty(Properties.Settings.Default.lastDirectory))
            {
                folderBrowserDialog1.SelectedPath = Properties.Settings.Default.lastDirectory;
            }
            if (!String.IsNullOrEmpty(Properties.Settings.Default.currentXMLTemplate))
            {
                string filePath = Properties.Settings.Default.currentXMLTemplate;

                //Read the contents of the file into a stream
                var fileStream = openFileDialog1.OpenFile();
                XmlSerializer ser = new XmlSerializer(typeof(CustomFunctions), new Type[] { typeof(AbsoluteSearchFunction), typeof(StringOccurenceFunction) });

                StreamReader rdr = new StreamReader(filePath);

                customFunctions = (CustomFunctions)ser.Deserialize(rdr);

                foreach (CustomFunction f in customFunctions.FunctionList)
                {
                    MessageBox.Show(f.ReadFunction("sadfadfasdfasdfasdfsdaf Enthalpies afasdfasdfasdfasdf adsfasdfsdafasdfsadfasdfasdfsdaf"));
                }
                updateBindings();
            }

            dataset = "";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                CustomFunction c = (CustomFunction)comboBox1.SelectedItem;
                textBox1.Text = c.ReadFunction("sadfasdfasdfasdf");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            processData();
            Form2 form = new Form2();
            form.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string s = processReport();
            Form3 form = new Form3();
            form.Show();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            string s = processReport();
            Form3 form = new Form3();
            form.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                if (listBox1.SelectedItem.ToString().EndsWith(".com") && Properties.Settings.Default.ComFileProgram)
                {
                    System.Diagnostics.Process.Start("notepad.exe", listBox1.SelectedItem.ToString());
                }
                else
                {
                    System.Diagnostics.Process.Start(listBox1.SelectedItem.ToString());
                }
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form5 form = new Form5();
            form.Show();
        }

        private void editAutomationTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void newAutomationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form6 form = new Form6(true);
            if (form.ShowDialog() == DialogResult.OK)
            {
                customFunctions = form.SessionTemplate;
            }
        }

        private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 form = new Form4();
            form.Show();
        }

        private void saveResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processData();

            var lines = new List<string>();
            string[] columnNames = valueTable.Columns
                .Cast<DataColumn>()
                .Select(column => column.ColumnName)
                .ToArray();

            var header = string.Join(",", columnNames.Select(name => $"\"{name}\""));
            lines.Add(header);

            var valueLines = valueTable.AsEnumerable()
           .Select(row => string.Join(",", row.ItemArray.Select(val => $"\"{val}\"")));

            lines.AddRange(valueLines);


            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(saveFileDialog1.FileName, lines);

            }
        }

        private void viewDatasetInGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processData();
            Form2 form = new Form2();
            form.Show();
        }

        private void openAutomationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                string filePath = openFileDialog1.FileName;
                Properties.Settings.Default.currentXMLTemplate = filePath;
                MessageBox.Show(Properties.Settings.Default.currentXMLTemplate);


                //Read the contents of the file into a stream
                var fileStream = openFileDialog1.OpenFile();
                XmlSerializer ser = new XmlSerializer(typeof(CustomFunctions), new Type[] { typeof(AbsoluteSearchFunction), typeof(StringOccurenceFunction) });

                StreamReader rdr = new StreamReader(filePath);

                customFunctions = (CustomFunctions)ser.Deserialize(rdr);

                
                updateBindings();
            }
        }
        private void updateBindings()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = customFunctions.FunctionList;
            checkedListBox1.DataSource = null;
            checkedListBox1.DataSource = bindingSource.DataSource;
            comboBox1.DataSource = null;
            comboBox1.DataSource = bindingSource.DataSource;


            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "Name";
            checkedListBox1.DataSource = bindingSource.DataSource;
            checkedListBox1.DisplayMember = "Name";
            checkedListBox1.ValueMember = "Name";

        }

        private void button4_Click_2(object sender, EventArgs e)
        {
            System.Reflection.MethodInfo meth = typeof(StringOccurenceFunction).GetMethod("Create");
            customFunctions.FunctionList.Add(CustomFunction.Build(meth));
            bindingSource.DataSource = null;

            bindingSource.DataSource = customFunctions.FunctionList;



        }
    }
}
