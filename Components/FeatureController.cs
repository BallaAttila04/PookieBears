/*
' Copyright (c) 2025 PookieBears
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System.Collections.Generic;
//using System.Xml;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search;

namespace PookieBears.Dnn.Dnn_PookieBears_HelloWorld.Components
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Controller class for Dnn_PookieBears_HelloWorld
    /// 
    /// The FeatureController class is defined as the BusinessController in the manifest file (.dnn)
    /// DotNetNuke will poll this class to find out which Interfaces the class implements. 
    /// 
    /// The IPortable interface is used to import/export content from a DNN module
    /// 
    /// The ISearchable interface is used by DNN to index the content of a module
    /// 
    /// The IUpgradeable interface allows module developers to execute code during the upgrade 
    /// process for a module.
    /// 
    /// Below you will find stubbed out implementations of each, uncomment and populate with your own data
    /// </summary>
    /// -----------------------------------------------------------------------------

    //uncomment the interfaces to add the support.
    public class FeatureController //: IPortable, ISearchable, IUpgradeable
    {


        #region Optional Interfaces

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ExportModule implements the IPortable ExportModule Interface
        /// </summary>
        /// <param name="ModuleID">The Id of the module to be exported</param>
        /// -----------------------------------------------------------------------------
        //public string ExportModule(int ModuleID)
        //{
        //string strXML = "";

        //List<Dnn_PookieBears_HelloWorldInfo> colDnn_PookieBears_HelloWorlds = GetDnn_PookieBears_HelloWorlds(ModuleID);
        //if (colDnn_PookieBears_HelloWorlds.Count != 0)
        //{
        //    strXML += "<Dnn_PookieBears_HelloWorlds>";

        //    foreach (Dnn_PookieBears_HelloWorldInfo objDnn_PookieBears_HelloWorld in colDnn_PookieBears_HelloWorlds)
        //    {
        //        strXML += "<Dnn_PookieBears_HelloWorld>";
        //        strXML += "<content>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDnn_PookieBears_HelloWorld.Content) + "</content>";
        //        strXML += "</Dnn_PookieBears_HelloWorld>";
        //    }
        //    strXML += "</Dnn_PookieBears_HelloWorlds>";
        //}

        //return strXML;

        //	throw new System.NotImplementedException("The method or operation is not implemented.");
        //}

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ImportModule implements the IPortable ImportModule Interface
        /// </summary>
        /// <param name="ModuleID">The Id of the module to be imported</param>
        /// <param name="Content">The content to be imported</param>
        /// <param name="Version">The version of the module to be imported</param>
        /// <param name="UserId">The Id of the user performing the import</param>
        /// -----------------------------------------------------------------------------
        //public void ImportModule(int ModuleID, string Content, string Version, int UserID)
        //{
        //XmlNode xmlDnn_PookieBears_HelloWorlds = DotNetNuke.Common.Globals.GetContent(Content, "Dnn_PookieBears_HelloWorlds");
        //foreach (XmlNode xmlDnn_PookieBears_HelloWorld in xmlDnn_PookieBears_HelloWorlds.SelectNodes("Dnn_PookieBears_HelloWorld"))
        //{
        //    Dnn_PookieBears_HelloWorldInfo objDnn_PookieBears_HelloWorld = new Dnn_PookieBears_HelloWorldInfo();
        //    objDnn_PookieBears_HelloWorld.ModuleId = ModuleID;
        //    objDnn_PookieBears_HelloWorld.Content = xmlDnn_PookieBears_HelloWorld.SelectSingleNode("content").InnerText;
        //    objDnn_PookieBears_HelloWorld.CreatedByUser = UserID;
        //    AddDnn_PookieBears_HelloWorld(objDnn_PookieBears_HelloWorld);
        //}

        //	throw new System.NotImplementedException("The method or operation is not implemented.");
        //}

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetSearchItems implements the ISearchable Interface
        /// </summary>
        /// <param name="ModInfo">The ModuleInfo for the module to be Indexed</param>
        /// -----------------------------------------------------------------------------
        //public DotNetNuke.Services.Search.SearchItemInfoCollection GetSearchItems(DotNetNuke.Entities.Modules.ModuleInfo ModInfo)
        //{
        //SearchItemInfoCollection SearchItemCollection = new SearchItemInfoCollection();

        //List<Dnn_PookieBears_HelloWorldInfo> colDnn_PookieBears_HelloWorlds = GetDnn_PookieBears_HelloWorlds(ModInfo.ModuleID);

        //foreach (Dnn_PookieBears_HelloWorldInfo objDnn_PookieBears_HelloWorld in colDnn_PookieBears_HelloWorlds)
        //{
        //    SearchItemInfo SearchItem = new SearchItemInfo(ModInfo.ModuleTitle, objDnn_PookieBears_HelloWorld.Content, objDnn_PookieBears_HelloWorld.CreatedByUser, objDnn_PookieBears_HelloWorld.CreatedDate, ModInfo.ModuleID, objDnn_PookieBears_HelloWorld.ItemId.ToString(), objDnn_PookieBears_HelloWorld.Content, "ItemId=" + objDnn_PookieBears_HelloWorld.ItemId.ToString());
        //    SearchItemCollection.Add(SearchItem);
        //}

        //return SearchItemCollection;

        //	throw new System.NotImplementedException("The method or operation is not implemented.");
        //}

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpgradeModule implements the IUpgradeable Interface
        /// </summary>
        /// <param name="Version">The current version of the module</param>
        /// -----------------------------------------------------------------------------
        //public string UpgradeModule(string Version)
        //{
        //	throw new System.NotImplementedException("The method or operation is not implemented.");
        //}

        #endregion

    }

}
