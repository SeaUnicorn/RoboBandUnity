using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/**
 * <summary> a class provides FTPconnection</summary>
 */
public class Connection : MonoBehaviour {

    /**
     * <value> adress, fileName - pointer to inputfields</value>
     * <value> tabs - a result string container</value>
     * <value> con (content) - array of fileNames from FTP server</value>
     * <value> Name - filename </value>
     */

    public GameObject adress;
    public GameObject LoadButton;
    public string tabs;
    public string[] con;
    private InputField fileName;
    private string Name;
    

    /** 
     * <value> Start - a method is called once // sets default values to inputfield and copies results</value>
     */
    public void Start()
    {
        adress.GetComponentInChildren<Text>().text = "192.168.10.10";
        tabs = GameObject.Find("Load_Tabs").GetComponent<Bass>().CMD;

        string path = "ftp://" + adress.GetComponentInChildren<Text>().text + "/CNC_Prg/";
        //load catalog from defoult adress
        Load_Catalog(path);
        Name = "";
    }

    /**
     * <value> Cancel - a method is called when user presses 'Cancel' button// returns previous menu</value>
     */
    public void Cancel()
    {
        gameObject.SetActive(false);
    }

    /**
     * <value> Connect_Upload - a method is called when user presses 'Cancel' button// conects to the server and uploads file</value>
     */
    public void Connect_Upload()
    {
        //ftp server path
        string path = "ftp://" + adress.GetComponentInChildren<Text>().text + "/CNC_Prg/";
        //user hasn't chosen name of file to save
        if (Name.Length == 0) con[0] = "Set Name To File, Please";
        else
        {
            string fileName = Name; //.PRG - AS program format

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path + Name);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            try
            {
                // Copy the contents of the file to the request stream.
                byte[] fileContents = Encoding.UTF8.GetBytes(tabs);

                request.ContentLength = (long)fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);

                path = "ftp://" + adress.GetComponentInChildren<Text>().text + "/CNC_Prg/";

                request = (FtpWebRequest)WebRequest.Create(path);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse responseCommon = (FtpWebResponse)request.GetResponse();

                request.Abort();
            }
            catch (Exception)
            {
                adress.GetComponentInChildren<Text>().text = "No connection";
                throw;
            }
        }
        //reload catalog
        Load_Catalog(path);

    }

    /**
     * <value> Load_Catalog - a separate method to load fileList from ftp</value>
     * <param name="path"> adress, string</param>
     */
    private void Load_Catalog(string path)
    {
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
        request.Method = WebRequestMethods.Ftp.ListDirectory;
        
        try
        {
            FtpWebResponse responseCommon = (FtpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(responseCommon.GetResponseStream());
            //all files in one string
            string catalog = sr.ReadToEnd();
            //splited string to con (content)
            con = catalog.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            sr.Close();
            responseCommon.Close();
            request.Abort();
        }
        catch (Exception)
        {
            adress.GetComponentInChildren<Text>().text = "No connection";
            throw;
        }

    }

    /**
     * <value> EditName - a method to change file name</value>
     * <param name="value"> string, new Name</param>
     */
    public void EditName(string value) => Name = value;

    /**
     * <value> LoadFromFTP - a method to load file from ftp </value>
     * <param name="value"> file name</param>
     */
    public void LoadFromFTP(string value)
    {
        //formulates adress
        string path = "ftp://" + adress.GetComponentInChildren<Text>().text + "/CNC_Prg/" + value;
        //sets connection
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
        request.Method = WebRequestMethods.Ftp.DownloadFile;
        try { 
            FtpWebResponse responseCommon = (FtpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(responseCommon.GetResponseStream());
            //read tabs from file
            tabs = sr.ReadToEnd();
            //sent tabs to previous menu
            LoadButton.GetComponent<Bass>().GetTabsFromFTP(tabs);
            //close connection
            sr.Close();
            responseCommon.Close();
            request.Abort();
        }
        catch (Exception)
        {
            adress.GetComponentInChildren<Text>().text = "No connection";
            throw;
        }
    }

    /**
     * <value> DeleteFromFTP - a method to delete file from server </value>
     * <param name="value"> file name</param>
     */
    public void DeleteFromFTP(string value)
    {
        //formulates adress
        string path = "ftp://" + adress.GetComponentInChildren<Text>().text + "/CNC_Prg/" + value;
        //sets connection
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
        //delete
        request.Method = WebRequestMethods.Ftp.DeleteFile;
        try
        {
            FtpWebResponse responseCommon = (FtpWebResponse)request.GetResponse();
            //close connection
            responseCommon.Close();
            request.Abort();
        }
        catch (Exception)
        {
            adress.GetComponentInChildren<Text>().text = "No connection";
            throw;
        }
        //reload catalog
        Load_Catalog(("ftp://" + adress.GetComponentInChildren<Text>().text + "/CNC_Prg/"));
    }
    
}
