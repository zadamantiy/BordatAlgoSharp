using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BordatSystem.ConceptLattices;

namespace BordatSystem
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private List<CLObject> _clObjects;
        private List<List<bool>> _objectAttributes;
        private List<string> _attributeNames;
        private List<string> _objectNames;

        private void LoadObjectsFromCxt()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Cxt files (*.cxt)|*.cxt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 3;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var path = openFileDialog.FileName.ToLower();

                    if (!File.Exists(path))
                        return;

                    var fName = openFileDialog.FileName.ToLower();

                    using (var reader = new StreamReader(fName))
                    {
                        reader.ReadLine();
                        reader.ReadLine();

                        //read number of objects
                        var objNumberString = reader.ReadLine();
                        int.TryParse(objNumberString, out var objNumber);

                        //read number of attributes
                        var attrNumberString = reader.ReadLine();
                        int.TryParse(attrNumberString, out var attrNumber);

                        //read all objects
                        _objectNames = new List<string>();
                        for (var i = 0; i < objNumber; i++)
                        {
                            var s = reader.ReadLine();
                            if (s == "")
                            {
                                i--;
                                continue;
                            }

                            _objectNames.Add(s);
                        }

                        //read all attributes
                        _attributeNames = new List<string>();
                        for (var i = 0; i < attrNumber; i++)
                        {
                            var s = reader.ReadLine();
                            if (s == "")
                            {
                                i--;
                                continue;
                            }
                            _attributeNames.Add(s);
                        }

                        //read all attributes values
                        _clObjects = new List<CLObject>();
                        _objectAttributes = new List<List<bool>>();
                        for (var i = 0; i < objNumber; i++)
                        {
                            var s = reader.ReadLine();
                            if (s == "")
                            {
                                i--;
                                continue;
                            }
                            var attrList = new List<char>();
                            _objectAttributes.Add(new List<bool>());
                            for (var j = 0; j < s.Length; j++)
                            {
                                var chr = s[j];
                                bool hasJthAttribute = chr == 'X';
                                _objectAttributes[i].Add(hasJthAttribute);
                                if (hasJthAttribute)
                                    attrList.Add((char)('a' + j));
                            }

                            _clObjects.Add(new CLObject(attrList, i + 1));
                        }
                    }
                }
            }
        }

        private void processButton_Click(object sender, EventArgs e)
        {
            //Just a Test
            /*var objs = new List<CLObject>()
            {
                new CLObject(new[] {'a', 'c', 'd'}, 1),
                new CLObject(new[] {'a', 'c'}, 2),
                new CLObject(new[] {'b', 'c'}, 3),
                new CLObject(new[] {'b', 'd'}, 4)
            };*/

            /*
            //Rusakov
            var objs = new List<CLObject>()
            {
                new CLObject(new char[]{}, 1),
                new CLObject(new[] {'a', 'c'}, 2),
                new CLObject(new[] {'c'}, 3),
            };*/

            /*
            //Zhivotovskaya's example
            var objs = new List<CLObject>()
            {
                new CLObject(new char[]{'a', 'b'}, 1),
                new CLObject(new[] {'a', }, 2),
                new CLObject(new[] {'a', 'b', 'c'}, 3),
            };*/

            if (_clObjects == null)
            { 
                MessageBox.Show("Данные не загружены", "Ошибка");
                return;
            }

            var b = new Bordat();
            var result = b.Proceed(_clObjects);

            //var x = b.DesiredConceptLattices;
            Log("[EXECUTION LOG]");
            Log(b.GetLog());

            var sb = new StringBuilder();
            sb.Append("[RESULT]\n");
            var idx = 1;
            foreach (var cl in result)
            {
                sb.Append($"{idx++}) {cl}\n");
            }
            Log(sb.ToString());
        }

        private void Log(string s)
        {
            logRichTextBox.Text += $"{s}\n";
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            LoadObjectsFromCxt();
            dataListView.Items.Clear();
            dataListView.Columns.Clear();
            dataListView.Columns.Add("Attribute");
            foreach (var attribute in _attributeNames)
            {
                dataListView.Columns.Add(attribute);
            }

            for (var i = 0; i < _objectNames.Count; i++)
            {
                var obj = _objectNames[i];
                var item = new ListViewItem(obj);
                foreach (var attr in _objectAttributes[i])
                    item.SubItems.Add(attr ? "x" : "");
                dataListView.Items.Add(item);
            }
        }

        private void clearLogButton_Click(object sender, EventArgs e)
        {
            logRichTextBox.Text = "";
        }
    }
}
