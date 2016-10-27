using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using Willowsoft.WillowLib.Data.Misc;

namespace Willowsoft.WillowLib.CodeGen
{
    public partial class CodeViewer : Form
    {
        private string mDefinitionsPath;

        public CodeViewer()
        {
            InitializeComponent();
        }

        public string DefinitionsPath
        {
            get { return mDefinitionsPath; }
            set { mDefinitionsPath = value; }
        }

        private XmlDocument GetDefinitions()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(mDefinitionsPath);
            return doc;
        }

        private void CodeViewer_Load(object sender, EventArgs e)
        {
            cboEntity.Items.Clear();
            XmlNodeList entities = GetDefinitions().DocumentElement.SelectNodes(DefConstants.EntityElement);
            foreach (XmlElement entity in entities)
            {
                cboEntity.Items.Add(entity.GetAttribute(DefConstants.EntityClassnameAttrib));
            }
        }

        private void btnGenerateClass_Click(object sender, EventArgs e)
        {
            if (cboEntity.SelectedItem == null)
            {
                MessageBox.Show("Please select class to generate");
                return;
            }
            string entityName = (string)cboEntity.SelectedItem;
            XmlDocument doc = GetDefinitions();
            XmlElement entityElement = (XmlElement)doc.DocumentElement.SelectSingleNode(
                string.Format("{0}[@{1}='{2}']", DefConstants.EntityElement,
                DefConstants.EntityClassnameAttrib, entityName));
            if (entityElement == null)
            {
                MessageBox.Show("Unable to find class name.");
                return;
            }
            StringWriter output = new StringWriter();
            ErrorList errors = new ErrorList();
            ClassCreator creator = new ClassCreator(output, errors);
            creator.OutputPartialClasses(entityElement);
            ShowResults(output, errors);
        }

        private void btnGenerateTable_Click(object sender, EventArgs e)
        {
            XmlDocument doc = GetDefinitions();
            StringWriter output = new StringWriter();
            ErrorList errors = new ErrorList();
            TableCreator creator = new TableCreator(output, errors);
            creator.OutputTableScript(doc.DocumentElement);
            ShowResults(output, errors);
        }

        private void btnGenerateConversion_Click(object sender, EventArgs e)
        {
            XmlDocument doc = GetDefinitions();
            StringWriter output = new StringWriter();
            ErrorList errors = new ErrorList();
            TableCreator creator = new TableCreator(output, errors);
            creator.OutputConversionScript(doc.DocumentElement);
            ShowResults(output, errors);
        }

        private void btnGenerate_Proc(object sender, EventArgs e)
        {
            XmlDocument doc = GetDefinitions();
            StringWriter output = new StringWriter();
            ErrorList errors = new ErrorList();
            StoredProcCreator creator = new StoredProcCreator(output, errors);
            creator.OutputProcScript(doc.DocumentElement);
            ShowResults(output, errors);
        }

        private void ShowResults(StringWriter output, ErrorList errors)
        {
            foreach (UserError error in errors)
            {
                output.WriteLine(error.Message);
            }
            txtOutput.Text = output.GetStringBuilder().ToString();
        }
    }
}
