using CreatorChannelsXrmToolbox.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace CreatorChannelsXrmToolbox
{
    /// <summary>
    /// This class allows general operations and handling of the solution XML
    /// </summary>
    public class GeneralOperations
    {
        /// <summary>
        /// Indicates whether the name of a message part is valid.
        /// </summary>
        /// <param name="name">Name of the message part</param>
        /// <returns>True indicating that it is valid and False, that it is invalid.</returns>
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length > 100)
                return false;

            return Regex.IsMatch(name, @"^[a-zA-Z0-9_-]+$");
        }

        /// <summary>
        /// Remove carriage returns and line breaks from a string.
        /// </summary>
        /// <param name="text">String to process</param>
        /// <returns>String processed without escape characters</returns>
        public static string RemoveLineBreaksRegex(string text)
        {
            if (text == null) return null;
            return Regex.Replace(text, @"[\r\n]+", "");
        }

        /// <summary>
        /// Creates the XML necessary to establish the message parts required for the channel
        /// </summary>
        /// <param name="document">Instantiated XML document, corresponds to the customizations.xml file of the solution worked on.</param>
        /// <param name="listMessageParts">List of the configured message parts</param>
        /// <returns>Generated XML element</returns>
        public static XmlElement CreateMessageParts(XmlDocument document, List<MessagePartData> listMessageParts)
        {
            XmlNode _node = document.SelectSingleNode("//msdyn_channelmessageparts");
            if (_node != null && _node.ParentNode != null)
                _node.ParentNode.RemoveChild(_node);

            // Root element
            XmlElement _rootNode = document.CreateElement("msdyn_channelmessageparts");
            document.DocumentElement.AppendChild(_rootNode);

            int _total = listMessageParts.Count;

            foreach (MessagePartData _item in listMessageParts)
            {
                int _orderNum = (_total + 1) - _item.Order.Value;
                //Main element of the message part.
                XmlElement _messagePart = document.CreateElement("msdyn_channelmessagepart");
                _messagePart.SetAttribute("msdyn_channelmessagepartid", _item.Id.ToString().ToLower());
                _rootNode.AppendChild(_messagePart);

                //Channel id element
                XmlElement _rootChannelId = document.CreateElement("msdyn_channeldefinitionid");
                XmlElement _channelId = document.CreateElement("msdyn_channeldefinitionid");
                _channelId.InnerText = _item.ChannelId.ToString().ToLower();
                _rootChannelId.AppendChild(_channelId);
                _messagePart.AppendChild(_rootChannelId);

                //Name element
                XmlElement _name = document.CreateElement("msdyn_name");
                _name.InnerText = _item.Name;
                _messagePart.AppendChild(_name);

                //Display Name Element
                XmlElement _displayName = document.CreateElement("msdyn_displayname");
                _displayName.InnerText = _item.DisplayName;
                _messagePart.AppendChild(_displayName);

                //Description element
                XmlElement _description = document.CreateElement("msdyn_description");
                _description.InnerText = _item.Description;
                _messagePart.AppendChild(_description);

                //Type element
                XmlElement _type = document.CreateElement("msdyn_type");
                _type.InnerText = _item.Type.ToString();
                _messagePart.AppendChild(_type);

                //Required element
                XmlElement _required = document.CreateElement("msdyn_isrequired");
                _required.InnerText = _item.Required ? "1" : "0";
                _messagePart.AppendChild(_required);

                //Max Length element
                XmlElement _length = document.CreateElement("msdyn_maxlength");
                _length.InnerText = _item.MaxLength.ToString();
                _messagePart.AppendChild(_length);

                //State element
                XmlElement _state = document.CreateElement("statecode");
                _state.InnerText = _item.State.ToString();
                _messagePart.AppendChild(_state);

                XmlElement _status = document.CreateElement("statuscode");
                _status.InnerText = _item.Status.ToString();
                _messagePart.AppendChild(_status);

                //Optional elements
                //Order element
                if (_item.Order.HasValue)
                {
                    XmlElement _order = document.CreateElement("msdyn_order");
                    _order.InnerText = _orderNum.ToString();
                    _messagePart.AppendChild(_order);
                }

                //Entity Id element
                if (!string.IsNullOrEmpty(_item.Entity))
                {
                    XmlElement _entity = document.CreateElement("msdyn_entityname");
                    _entity.InnerText = _item.Entity;
                    _messagePart.AppendChild(_entity);
                }

                //View id element
                if (_item.ViewId != Guid.Empty)
                {
                    XmlElement _view = document.CreateElement("msdyn_entityviewid");
                    _view.InnerText = _item.ViewId.ToString().ToLower();
                    _messagePart.AppendChild(_view);
                }
            }
            return _rootNode;
        }

        /// <summary>
        /// Creates the XML needed to set the channel locale, to display descriptions and tags in different languages
        /// </summary>
        /// <param name="document">Instantiated XML document, corresponds to the customizations.xml file of the solution worked on.</param>
        /// <param name="listLocales">List of locales with different labels by language</param>
        /// <returns>Generated XML element</returns>
        public static XmlElement CreateLocalization(XmlDocument document, List<Locale> listLocales)
        {
            XmlNode _node = document.SelectSingleNode("//msdyn_channeldefinitionlocales");
            if (_node != null && _node.ParentNode != null)
                _node.ParentNode.RemoveChild(_node);

            // Root node element
            XmlElement _rootNode = document.CreateElement("msdyn_channeldefinitionlocales");
            document.DocumentElement.AppendChild(_rootNode);

            foreach (Locale _item in listLocales)
            {
                //Localization element
                XmlElement _localization = document.CreateElement("msdyn_channeldefinitionlocale");
                _localization.SetAttribute("msdyn_channeldefinitionlocaleid", _item.Id.ToString().ToLower());
                _rootNode.AppendChild(_localization);

                //Channel Id element
                XmlElement _channelId = document.CreateElement("msdyn_channeldefinitionid");
                _channelId.InnerText = _item.ChannelId.ToString().ToLower();
                _localization.AppendChild(_channelId);

                //Language Id element
                XmlElement _languageId = document.CreateElement("msdyn_localeid");
                _languageId.InnerText = _item.LanguageId.ToString();
                _localization.AppendChild(_languageId);

                //Content element
                XmlElement _contentJSON = document.CreateElement("msdyn_localecontent");
                _contentJSON.InnerText = _item.JSONContent;
                _localization.AppendChild(_contentJSON);
            }
            return _rootNode;
        }

        /// <summary>
        /// Creates the XML needed to set the channel information.
        /// </summary>
        /// <param name="document">Instantiated XML document, corresponds to the customizations.xml file of the solution worked on.</param>
        /// <param name="channelInfo">Channel data to be generated</param>
        /// <returns>Generated XML element</returns>
        public static XmlElement CreateChannel(XmlDocument document, ChannelXmlData channelInfo)
        {
            XmlNode _node = document.SelectSingleNode("//msdyn_channeldefinitions");
            if (_node != null && _node.ParentNode != null)
                _node.ParentNode.RemoveChild(_node);

            // Root element
            XmlElement _rootNode = document.CreateElement("msdyn_channeldefinitions");
            document.DocumentElement.AppendChild(_rootNode);

            //Channel element
            XmlElement _channel = document.CreateElement("msdyn_channeldefinition");
            _channel.SetAttribute("msdyn_channeldefinitionid", channelInfo.Id.ToString().ToLower());
            _rootNode.AppendChild(_channel);

            //Configuration entity element
            XmlElement _configurationEntity = document.CreateElement("msdyn_channeldefinitionexternalentity");
            _configurationEntity.InnerText = channelInfo.ConfigurationEntity;
            _channel.AppendChild(_configurationEntity);

            //Configuration form element
            XmlElement _configurationForm = document.CreateElement("msdyn_channeldefinitionexternalformid");
            _configurationForm.InnerText = channelInfo.ConfigurationForm.ToString().ToLower();
            _channel.AppendChild(_configurationForm);

            if (channelInfo.AccountEntity != null && channelInfo.Type.Equals("SMS"))
            {
                //Account entity element
                XmlElement _accountEntity = document.CreateElement("msdyn_channeldefinitionaccountexternalentity");
                _accountEntity.InnerText = channelInfo.AccountEntity;
                _channel.AppendChild(_accountEntity);

                //Account form element
                XmlElement _accountForm = document.CreateElement("msdyn_channeldefinitionaccountexternalformid");
                _accountForm.InnerText = channelInfo.AccountForm.ToString().ToLower();
                _channel.AppendChild(_accountForm);
            }

            //Type element
            XmlElement _type = document.CreateElement("msdyn_channeltype");
            _type.InnerText = channelInfo.Type;
            _channel.AppendChild(_type);

            //Description element
            XmlElement _description = document.CreateElement("msdyn_description");
            _description.InnerText = channelInfo.Description;
            _channel.AppendChild(_description);

            //Display name element
            XmlElement _displayName = document.CreateElement("msdyn_displayname");
            _displayName.InnerText = channelInfo.DisplayName;
            _channel.AppendChild(_displayName);

            //Allow delivery element
            XmlElement _delivery = document.CreateElement("msdyn_hasdeliveryreceipt");
            _delivery.InnerText = channelInfo.AllowDelivery ? "1" : "0";
            _channel.AppendChild(_delivery);

            //Allow inbound element
            XmlElement _inbound = document.CreateElement("msdyn_hasinbound");
            _inbound.InnerText = channelInfo.AllowInbound ? "1" : "0";
            _channel.AppendChild(_inbound);

            if (channelInfo.EditorForm != Guid.Empty)
            {
                //Editor form element
                XmlElement _editorForm = document.CreateElement("msdyn_messageformid");
                _editorForm.InnerText = channelInfo.EditorForm.ToString();
                _channel.AppendChild(_editorForm);
            }

            //Custom API element
            XmlElement _customAPI = document.CreateElement("msdyn_outboundendpointurltemplate");
            _customAPI.InnerText = channelInfo.EndpointOutput;
            _channel.AppendChild(_customAPI);

            //Special consent element
            XmlElement _specialConsent = document.CreateElement("msdyn_specialconsentrequired");
            _specialConsent.InnerText = channelInfo.RequiresSpecialConsent ? "1" : "0";
            _channel.AppendChild(_specialConsent);

            //Allow accounts
            XmlElement _accounts = document.CreateElement("msdyn_supportsaccount");
            _accounts.InnerText = channelInfo.AllowAccounts ? "1" : "0";
            _channel.AppendChild(_accounts);

            //Allow attachment
            XmlElement _attachment = document.CreateElement("msdyn_supportsattachment");
            _attachment.InnerText = channelInfo.AllowAttachment ? "1" : "0";
            _channel.AppendChild(_attachment);

            //Allow binary element
            XmlElement _binary = document.CreateElement("msdyn_supportsbinary");
            _binary.InnerText = channelInfo.AllowBinary ? "1" : "0";
            _channel.AppendChild(_binary);

            //State element
            XmlElement _state = document.CreateElement("statecode");
            _state.InnerText = channelInfo.State.ToString();
            _channel.AppendChild(_state);

            //Status element
            XmlElement _status = document.CreateElement("statuscode");
            _status.InnerText = channelInfo.Status.ToString();
            _channel.AppendChild(_status);

            return _rootNode;
        }

        /// <summary>
        /// Instantiated XML document, corresponds to the customizations.xml file of the solution worked on.
        /// </summary>
        /// <param name="extractionPath">Path where the exported solution was extracted and where the customizations.xml file is located</param>
        /// <param name="solutionName">Solution Name/param>
        /// <returns>Instance to the XML document</returns>
        public static XmlDocument GetDocument(string extractionPath, string solutionName)
        {
            string _tempPath = extractionPath + "\\" + Path.GetFileNameWithoutExtension(solutionName);
            string _pathFile = Path.Combine(_tempPath, "customizations.xml");
            XmlDocument _document = new XmlDocument();
            _document.Load(_pathFile);
            return _document;
        }
        /// <summary>
        /// Save the changes to the customizations.xml file with the new channel data.
        /// </summary>
        /// <param name="document">Instantiated XML document, corresponds to the customizations.xml file of the solution worked on.</param>
        /// <param name="extractionPath">Path where the exported solution was extracted and where the customizations.xml file is located</param>
        /// <param name="solutionName">Solution name</param>
        /// <exception cref="Exception">Exception if the new file cannot be saved</exception>
        public static void EditXMLCustomization(XmlDocument document, string extractionPath, string solutionName)
        {
            string _tempPath = extractionPath + "\\" + Path.GetFileNameWithoutExtension(solutionName);
            string _pathFile = Path.Combine(_tempPath, "customizations.xml");

            // Search node "ImportExportXml"
            XmlNode _node = document.SelectSingleNode("//ImportExportXml");

            if (_node != null)
                document.Save(_pathFile);
            else
                throw new Exception("The ImportExportXml node could not be found in the customizations.xml file.");
        }
    }
}
