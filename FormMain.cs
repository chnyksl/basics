using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace EFaturaGoruntuleyici
{
    public partial class FormMain : Form
    {
        Encoding en;
        string strXLST = "";
        private OpenFileDialog openFileDialog1;

        public FormMain()
        {
            InitializeComponent();
            openFileDialog1 = new OpenFileDialog();
            en = Encoding.UTF8;
            string startup_path = Application.StartupPath;
            string xlst_path = startup_path + "\\general.xslt";
            if (!File.Exists(xlst_path))
            {
                MessageBox.Show(xlst_path + " dosyası bulunamadı");
                return;
            }
            else
            {
                strXLST = File.ReadAllText(xlst_path, en);
            }
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (!openFileDialog1.FileName.ToLower().EndsWith(".xml"))
                    {
                        MessageBox.Show("sadece xml uzantılı dosyalar desteklenmektedir");
                        return;
                    }
                    var sr = new StreamReader(openFileDialog1.FileName);
                    txtFilePath.Text = openFileDialog1.FileName;
                    string strFaturaXML = sr.ReadToEnd();

                    string ErrMsg = "";
                    string strResult = Transform(strFaturaXML, strXLST, out ErrMsg);
                    
                    if (ErrMsg != "")
                        MessageBox.Show(ErrMsg);
                    else
                    {
                        webBrowser1.DocumentText = strResult;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error message: " + ex.Message + Environment.NewLine + "Details:" + ex.StackTrace);
                }
            }
        }

        private void btnYazdir_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintDialog();
        }


        private string Transform(string XMLPage, string XSLStylesheet, out string ErrorMessage)
        {

            string result = "";
            ErrorMessage = "";
            try
            {
                // Reading XML
                TextReader textReader1 = new StringReader(XMLPage);
                XmlTextReader xmlTextReader1 = new XmlTextReader(textReader1);
                XPathDocument xPathDocument = new XPathDocument(xmlTextReader1);

                //Reading XSLT
                TextReader textReader2 = new StringReader(XSLStylesheet);
                XmlTextReader xmlTextReader2 = new XmlTextReader(textReader2);

                //Define XslCompiledTransform
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(xmlTextReader2);


                StringBuilder sb = new StringBuilder();
                TextWriter tw = new StringWriter(sb);

                xslt.Transform(xPathDocument, null, tw);

                result = sb.ToString();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return result;
        }

    }
}
