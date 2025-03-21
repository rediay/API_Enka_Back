using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.FirmaNetco
{
    public class SignerFileSettings
    {
        public bool appCode { get; set; }
        public bool transferOperations { get; set; }
    }

    public class Metadata
    {
        public int id { get; set; }
        public int metadataID { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }

    public class DocumentRequest
    {
        public bool remind { get; set; }
        public bool combinePDF { get; set; }
        public bool sendConfirmation { get; set; }
        public bool ordered { get; set; }
        public bool locate { get; set; }
        public bool sendAll { get; set; }
        public bool multipleWorkflow { get; set; }
        public List<string> usersToPut { get; set; }
        public string optionName { get; set; }
        public int whoSigns { get; set; }
        public Dictionary<string, SignerFileSettings> signerFileSettings { get; set; }
        public int numTransfer { get; set; }
        public string templateIds { get; set; }
        public string base64FileBytes { get; set; }
        public string fileName { get; set; }
        public List<Metadata> metadata { get; set; }
        public string passwords { get; set; }
    }


    public class PdfRequest
    {
        public  bool returnBase64File { get; set; }
        public string uid { get; set; }

    }

}
