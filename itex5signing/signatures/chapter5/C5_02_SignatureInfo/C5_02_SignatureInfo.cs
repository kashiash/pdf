/*
 * This class is part of the white paper entitled
 * "Digital Signatures for PDF documents"
 * written by Bruno Lowagie
 * 
 * For more info, go to: http://itextpdf.com/learn
 */

using System;
using System.Collections.Generic;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.X509;
using iTextSharp.text;
using iTextSharp.text.log;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Newtonsoft.Json;

namespace signatures.chapter5
{
    class C5_02_SignatureInfo : C5_01_SignatureIntegrity
    {
        public const String EXAMPLE1 = @"c:\Users\jkosinski\source\repos\pdf\I8_V2_G47.pdf";
        public const String EXAMPLE2 = @"c:\Users\jkosinski\source\repos\pdf\Synchronizacja XL.pdf";
        public const String EXAMPLE3 = @"c:\Users\jkosinski\source\repos\pdf\I153_V4_G620.pdf";
        public const String EXAMPLE4 = "../../../../results/chapter2/hello_signed4.pdf";
        public const String EXAMPLE5 = "../../../../results/chapter4/hello_smartcard_Signature.pdf";
        public const String EXAMPLE6 = "../../../../results/chapter2/field_metadata.pdf";

        public SignaturePermissions InspectSignature(AcroFields fields, String name, SignaturePermissions perms)
        {
            IList<AcroFields.FieldPosition> fps = fields.GetFieldPositions(name);
            if (fps != null && fps.Count > 0)
            {
                AcroFields.FieldPosition fp = fps[0];
                Rectangle pos = fp.position;
                if (pos.Width == 0 || pos.Height == 0)
                {
                    Console.WriteLine("Invisible signature");
                }
                else
                {
                    Console.WriteLine("Field on page {0}; llx: {1}, lly: {2}, urx: {3}; ury: {4}",
                        fp.page, pos.Left, pos.Bottom, pos.Right, pos.Top);
                }
            }

            PdfPKCS7 pkcs7 = VerifySignature(fields, name);
            Console.WriteLine("Digest algorithm: " + pkcs7.GetHashAlgorithm());
            Console.WriteLine("Encryption algorithm: " + pkcs7.GetEncryptionAlgorithm());
            Console.WriteLine("Filter subtype: " + pkcs7.GetFilterSubtype());
            X509Certificate cert = pkcs7.SigningCertificate;
            Console.WriteLine("Name of the signer: " + CertificateInfo.GetSubjectFields(cert).GetField("CN"));
            if (pkcs7.SignName != null)
                Console.WriteLine("Alternative name of the signer: " + pkcs7.SignName);

            Console.WriteLine("Signed on: " + pkcs7.SignDate.ToString("yyyy-MM-dd HH:mm:ss.ff"));
            if (!pkcs7.TimeStampDate.Equals(DateTime.MaxValue))
            {
                Console.WriteLine("TimeStamp: " + pkcs7.TimeStampDate.ToString("yyyy-MM-dd HH:mm:ss.ff"));
                TimeStampToken ts = pkcs7.TimeStampToken;
                Console.WriteLine("TimeStamp service: " + ts.TimeStampInfo.Tsa);
                Console.WriteLine("Timestamp verified? " + pkcs7.VerifyTimestampImprint());
            }
            Console.WriteLine("Location: " + pkcs7.Location);
            Console.WriteLine("Reason: " + pkcs7.Reason);
            PdfDictionary sigDict = fields.GetSignatureDictionary(name);
            PdfString contact = sigDict.GetAsString(PdfName.CONTACTINFO);
            if (contact != null)
                Console.WriteLine("Contact info: " + contact);
            perms = new SignaturePermissions(sigDict, perms);
            Console.WriteLine("Signature type: " + (perms.Certification ? "certification" : "approval"));
            Console.WriteLine("Filling out fields allowed: " + perms.FillInAllowed);
            Console.WriteLine("Adding annotations allowed: " + perms.AnnotationsAllowed);
            foreach (SignaturePermissions.FieldLock Lock in perms.FieldLocks)
            {
                Console.WriteLine("Lock: " + Lock);
            }
            return perms;
        }

        public void InspectSignatures(String path)
        {
            Console.WriteLine(path);
            PdfReader reader = new PdfReader(path);
            AcroFields fields = reader.AcroFields;
            List<String> names = fields.GetSignatureNames();
            SignaturePermissions perms = null;
            foreach (String name in names)
            {
                Console.WriteLine("===== " + name + " =====");
                perms = InspectSignature(fields, name, perms);
            }
            Console.WriteLine();
        }

        static void Main(String[] args)
        {
            LoggerFactory.GetInstance().SetLogger(new SysoLogger());
            C5_02_SignatureInfo app = new C5_02_SignatureInfo();
          //  app.InspectSignatures(EXAMPLE1);
            var list = app.InspectSignatures2(EXAMPLE1);
            var json = JsonConvert.SerializeObject(list);
            Console.Write(json);
            var list2 = app.InspectSignatures2(EXAMPLE2);

            var json2 = JsonConvert.SerializeObject(list2);
            Console.Write(json2);
            var list3 = app.InspectSignatures2(EXAMPLE3);

            var json3 = JsonConvert.SerializeObject(list3);
            Console.Write(json3);
            //app.InspectSignatures(EXAMPLE4);
            //app.InspectSignatures(EXAMPLE5);
            //app.InspectSignatures(EXAMPLE6);
            Console.ReadLine();
        }

        private List<SignatureInfo> InspectSignatures2(string path)
        {
            Console.WriteLine(path);
            PdfReader reader = new PdfReader(path);
            AcroFields fields = reader.AcroFields;
            List<String> names = fields.GetSignatureNames();
            SignaturePermissions perms = null;
            var signatureInfoList = new List<SignatureInfo>();
            foreach (String name in names)
            {
                perms = GetSignatureInfo(fields, name, perms, signatureInfoList);
            }
            Console.WriteLine();
            return signatureInfoList;
        }

        private SignaturePermissions GetSignatureInfo(AcroFields fields, string name, SignaturePermissions perms, List<SignatureInfo> signatureInfoList)
        {

            var si = new SignatureInfo();

            PdfPKCS7 pkcs7 = fields.VerifySignature(name);

            X509Certificate cert = pkcs7.SigningCertificate;
            PdfDictionary sigDict = fields.GetSignatureDictionary(name);
            PdfString contact = sigDict.GetAsString(PdfName.CONTACTINFO);
            if (contact != null)
                Console.WriteLine("Contact info: " + contact);


            si.Signer = CertificateInfo.GetSubjectFields(cert).GetField("CN");
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

        class SignatureInfo
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
