﻿using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Net.Mail;
using System.Diagnostics;
using System.ComponentModel;

/// <summary>
/// Summary description for VCM_Mail
/// </summary>
public class VCM_Mail
{
    public VCM_Mail()
    { }
    
    private MailMessage _mailMessage;
    private SmtpClient _smptpClient;

    private string _mailTo, _mailCC, _mailBCC, _mailFrom, _mailSubject, _mailBody, _fileNameToAttach;
    bool _bSendAsync, _bIsBodyHtml;

    public static string strAmazonServer = System.Configuration.ConfigurationManager.AppSettings["aws_SMTPServer"].Trim();
    public static string strAmazonUserName = System.Configuration.ConfigurationManager.AppSettings["aws_UserId"].Trim();
    public static string strAmazonPassword = System.Configuration.ConfigurationManager.AppSettings["aws_Password"].Trim();
    public static int iPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["aws_Port"].Trim());

    public string To
    {
        get
        {
            return _mailTo;
        }
        set
        {
            _mailTo = value;
        }
    }

    public string CC
    {
        get
        {
            return _mailCC;
        }
        set
        {
            _mailCC = value;
        }
    }

    public string BCC
    {
        get
        {
            return _mailBCC;
        }
        set
        {
            _mailBCC = value;
        }
    }

    public string From
    {
        get
        {
            return _mailFrom;
        }
        set
        {
            _mailFrom = value;
        }
    }

    public string Subject
    {
        get
        {
            return _mailSubject;
        }
        set
        {
            _mailSubject = value;
        }
    }

    public string Body
    {
        get
        {
            return _mailBody;
        }
        set
        {
            _mailBody = value;
        }
    }

    public string FileToAttach
    {
        get
        {
            return _fileNameToAttach;
        }
        set
        {
            _fileNameToAttach = value;
        }
    }

    public bool SendAsync
    {
        get
        {
            return _bSendAsync;
        }
        set
        {
            _bSendAsync = value;
        }
    }

    public bool IsBodyHtml
    {
        get
        {
            return _bIsBodyHtml;
        }
        set
        {
            _bIsBodyHtml = value;
        }
    }

    public void SendMail()
    {
        try
        {
            _mailMessage = new MailMessage(_mailFrom, _mailTo);
            if (!string.IsNullOrEmpty(_mailCC)) { _mailMessage.CC.Add(_mailCC); }
            if (!string.IsNullOrEmpty(_mailBCC)) { _mailMessage.Bcc.Add(_mailBCC); }

            _mailMessage.Subject = _mailSubject;
            _mailMessage.Body = _mailBody;
            _mailMessage.IsBodyHtml = _bIsBodyHtml;

            if (!string.IsNullOrEmpty(_fileNameToAttach))
            {
                string strFileName = HttpContext.Current.Request.PhysicalApplicationPath.ToString() + "\\NewsLetters\\" + _fileNameToAttach;
                Attachment itm = new Attachment(strFileName);
                _mailMessage.Attachments.Add(itm);
            }

            //string strSMTPServer = System.Configuration.ConfigurationManager.AppSettings["SMTPSERVER"].ToString();
            //_smptpClient = new SmtpClient(strSMTPServer);
            _smptpClient = new SmtpClient();

            if (System.Configuration.ConfigurationManager.AppSettings["SystemRunningOn"].ToLower() == "amazon")
            {
                _smptpClient.Host = strAmazonServer;
                _smptpClient.EnableSsl = true;
                _smptpClient.Port = iPort;
                _smptpClient.Credentials = new System.Net.NetworkCredential(strAmazonUserName, strAmazonPassword);
            }
            else
            {
                _smptpClient.Host = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"];
            }

            if (_bSendAsync)
            {
                _smptpClient.SendCompleted += new SendCompletedEventHandler(_smptpClient_SendCompleted);
                _smptpClient.SendAsync(_mailMessage, "VCMmail");
            }
            else
            {
                _smptpClient.Send(_mailMessage);
            }
        }
        catch
        {
            throw;
        }
        finally
        {
            _mailMessage = null;
            _smptpClient = null;
        }
    }

    private void _smptpClient_SendCompleted(object sender, AsyncCompletedEventArgs e)
    {
    }

}
