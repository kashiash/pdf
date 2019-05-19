using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;

namespace iTextLib
{
    public class ITextLib
    {

        public List<SignatureInfo> InspectSignatures(string path)
        {
       
            PdfReader reader = new PdfReader(path);
            AcroFields fields = reader.AcroFields;
            List<String> names = fields.GetSignatureNames();
            SignaturePermissions perms = null;
            var signatureInfoList = new List<SignatureInfo>();
            foreach (String name in names)
            {
                perms = GetSignatureInfo(fields, name, perms, signatureInfoList);
            }
            return signatureInfoList;
        }

        private SignaturePermissions GetSignatureInfo(AcroFields fields, string name, SignaturePermissions perms, List<SignatureInfo> signatureInfoList)
        {

            var si = new SignatureInfo();

            PdfPKCS7 pkcs7 = fields.VerifySignature(name);

            X509Certificate cert = pkcs7.SigningCertificate;
            PdfDictionary sigDict = fields.GetSignatureDictionary(name);
            PdfString contact = sigDict.GetAsString(PdfName.CONTACTINFO);

            si.Signer = CertificateInfo.GetSubjectFields(cert).GetField("CN");

            if (contact != null)
            {
                si.ContactInfo = contact.ToString();
            }

            si.SignedOn = pkcs7.SignDate;
            si.Location = pkcs7.Location;
            si.Issuer = cert.IssuerDN.ToString();
            si.Subject = cert.SubjectDN.ToString();
            si.CertValidFrom = cert.NotBefore;
            si.CertValidTo = cert.NotAfter;
            si.Reason = pkcs7.Reason;
            si.IntegrityCheck = pkcs7.Verify();
            signatureInfoList.Add(si);

            perms = new SignaturePermissions(sigDict, perms);

            return perms;
        }

     public   class SignatureInfo
        {
            public string Issuer { get; set; }
            public string Subject { get; set; }
            public DateTime CertValidFrom { get; set; }
            public DateTime CertValidTo { get; set; }
            public DateTime SignedOn { get; set; }
            public string Reason { get; set; }
            public string Location { get; set; }
            public string Signer { get; set; }
            public string ContactInfo { get; set; }
            public bool IntegrityCheck { get; set; }

        }
    }
}
