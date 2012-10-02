/*
 * This class is part of the book "iText in Action - 2nd Edition"
 * written by Bruno Lowagie (ISBN: 9781935182610)
 * For more info, go to: http://itextpdf.com/examples/
 * This example only works with the AGPL version of iText.
 */

using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;

/**
 * Writing one PDF file to an output stream.
 * This is a port of java HelloZip sample. 
 * But since C# does not have native support for ZipOutputStream the sample operates with FileStream.
 */
namespace HelloWorldOutputStream
{
    class HelloWorldOutputStream
    {
        /** Path to the resulting PDF file. */
        public const String RESULT = "../../../../../results/part1/chapter01/hello_output_stream.pdf";

        /**
         * Creates a pdf file ending with "%HelloWorldOutputStream" comment.
         */
        static void Main(String[] args)
        {
            Document document = new Document();
            // step 2
            FileStream fs = new FileStream(RESULT, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            writer.CloseStream = false;
            // step 3
            document.Open();
            // step 4
            document.Add(new Paragraph("Hello World!"));
            // step 5
            document.Close();
            string str = "\n%HelloWorldOutputStream";
            byte [] bytes = new UTF8Encoding().GetBytes(str);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }
    }
}
