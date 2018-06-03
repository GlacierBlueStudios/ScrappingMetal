using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class mono_gmail : MonoBehaviour
{
    public GameObject  text;
    void Start()
    {
        //mailSend();
    }
    void OnMouseDown()
    {
        MailMessage mail = new MailMessage();

        mail.From = new MailAddress("mail@gmail.com", "From Unity");
        mail.To.Add("mail@gmail.com");   //acess the mobile user mail
        mail.Subject = "Test Mail";
        mail.Body = "This is for testing SMTP mail from GMAIL"+" at "+ System.DateTime.Now.ToString("MM/dd/yyyy"); 

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com",587);
        // smtpServer.Port = 587;
         smtpServer.Credentials = new System.Net.NetworkCredential("Mail@gmail.com", "*******") as ICredentialsByHost;
        //smtpServer.Credentials = CredentialCache.DefaultNetworkCredentials as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
        smtpServer.Send(mail);
        Debug.Log("success");
        text.SetActive( true);
    }
}