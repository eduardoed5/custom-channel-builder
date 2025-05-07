using System;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using CreatorChannelsXrmToolbox.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xrm.Sdk.Messages;
using System.Xml;
using MessageBox = System.Windows.Forms.MessageBox;
using XrmToolBox.Extensibility.Interfaces;

namespace CreatorChannelsXrmToolbox
{
    /// <summary>
    /// Plugin control class
    /// </summary>
    public partial class MyCustomChannelControl : PluginControlBase, IAboutPlugin, IPayPalPlugin
    {
        private Settings mySettings;
        private Guid _generatedChannelId = Guid.Empty;
        private Guid _generatedLocaleIdALT = Guid.Empty;
        private Guid _generatedLocaleIdENG = Guid.Empty;
        private bool _processComboChanges = true;
        private string _generatedSolution;
        private string TempPath;
        public string DonationDescription => "Donate";
        public string EmailAccount => "eric.902@hotmail.com";

        /// <summary>
        /// Class constructor 
        /// </summary>
        public MyCustomChannelControl()
        {
            InitializeComponent();
            //Default values ​​are loaded
            ListMessageType.Items.Add(new MessageType() { Id = 192350000, Name = "Text" });
            ListMessageType.Items.Add(new MessageType() { Id = 192350001, Name = "Html" });
            ListMessageType.Items.Add(new MessageType() { Id = 192350002, Name = "Url" });
            ListMessageType.Items.Add(new MessageType() { Id = 192350003, Name = "File" });
            ListMessageType.Items.Add(new MessageType() { Id = 192350004, Name = "Image" });
            ListMessageType.Items.Add(new MessageType() { Id = 192350005, Name = "Lookup" });

            ControlsPartsDisabled();

            _generatedChannelId = Guid.NewGuid();
            _generatedLocaleIdALT = Guid.NewGuid();
            _generatedLocaleIdENG = Guid.NewGuid();
            _processComboChanges = true;

        }

        /// <summary>
        /// Disables message part controls
        /// </summary>
        private void ControlsPartsDisabled()
        {
            TxtPartMessageName.Enabled = false;
            CmbPartMessageEntity.Enabled = false;
            CmbPartMessageView.Enabled = false;
            CheckPartMessageRequired.Enabled = false;
            BtnAddPartMessage.Enabled = false;
            TxtPartMessageDisplayName.Enabled = false;
            TxtPartMessageDescription.Enabled = false;
            NumMaxLength.Enabled = false;

            CheckPartMessageENG.Enabled = false;
            CheckPartMessageALT.Enabled = false;
            TxtPartDisplayNameALT.Enabled = false;
            TxtPartDescriptionALT.Enabled = false;
            TxtPartDisplayNameENG.Enabled = false;
            TxtPartDescriptionENG.Enabled = false;
        }

        /// <summary>
        /// Disble all control
        /// </summary>
        /// <param name="control">Main control</param>
        private void DisableAllControls(Control control)
        {
            foreach (Control _item in control.Controls)
            {
                if (!(_item is ToolStrip) && !(_item is System.Windows.Forms.Label))
                    _item.Enabled = false;
            }
        }

        /// <summary>
        /// Enabled all control
        /// </summary>
        /// <param name="control">Main control</param>
        private void EnableAllControls(Control control)
        {
            if (control != null)
            {
                foreach (Control _item in control.Controls)
                {
                    if (!(_item is ToolStrip) && !(_item is System.Windows.Forms.Label))
                        _item.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Create the temporary folder for the necessary files
        /// </summary>
        private void CreateTempPath()
        {
            //string _appDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\Plugins\\";//only Test
            string _appDirectory = Path.GetTempPath();
            // string _tempDirectory = Path.Combine(_appDirectory, "CreadorCanalesXrmToolbox");//only Test
            string _tempDirectory = Path.Combine(_appDirectory, "CreadorCanalesXrmToolbox");
            TempPath = _tempDirectory;
            if (!Directory.Exists(_tempDirectory))
                Directory.CreateDirectory(_tempDirectory);
        }

        /// <summary>
        /// Load the list of publishers
        /// </summary>
        /// <param name="list">List of publishers</param>
        private void LoadPublishers(List<Publisher> list)
        {
            CmbPublisher.Items.Clear();
            foreach (Publisher _item in list)
            {
                CmbPublisher.Items.Add(_item);
            }
        }

        /// <summary>
        /// Load the list of custom APIS
        /// </summary>
        /// <param name="list">List of custom APIS</param>
        private void LoadCustomAPIs(List<CustomAPI> list)
        {
            CmbCustomAPI.Items.Clear();
            foreach (CustomAPI _item in list)
            {
                CmbCustomAPI.Items.Add(_item);
            }
        }

        /// <summary>
        /// Load the list of entities
        /// </summary>
        /// <param name="list">List of entities</param>
        private void LoadEntities(List<EntityData> list)
        {
            CmbConfigEntity.Items.Clear();
            CmbPartMessageEntity.Items.Clear();
            CmbEditorEntity.Items.Clear();
            CmbEntityAdditional.Items.Clear();

            foreach (EntityData _item in list)
            {
                CmbConfigEntity.Items.Add(_item);
                CmbPartMessageEntity.Items.Add(_item);
                CmbEditorEntity.Items.Add(_item);
                CmbEntityAdditional.Items.Add(_item);
            }
        }

        /// <summary>
        /// Load the list of languages
        /// </summary>
        /// <param name="list">List of languages</param>
        private void LoadLanguages(List<LanguageData> list)
        {
            int _lcid = 3082;
            if (mySettings != null && mySettings.Lcid.HasValue)
                _lcid = mySettings.Lcid.Value;

            LanguageData _selected = null;

            CmbLanguage.Items.Clear();
            foreach (LanguageData _item in list)
            {
                if (_lcid == _item.Lcid)
                    _selected = _item;

                if (_item.Lcid != 1033)
                    CmbLanguage.Items.Add(_item);
                else
                {
                    CheckENG.Text = _item.DisplayName;
                    CheckPartMessageENG.Text = _item.DisplayName;
                }
            }
            CmbLanguage.SelectedItem = _selected;
        }

        /// <summary>
        /// Loads the necessary initial data
        /// </summary>
        private void LoadData()
        {
            DisableAllControls(this);
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Reading the initial elements...",
                Work = (worker, eventWorker) =>
                {
                    ResponseCRMOperations _response = new ResponseCRMOperations();
                    try
                    {
                        worker.ReportProgress(-1, "Checking temporary directory...");
                        CreateTempPath();
                        worker.ReportProgress(-1, "Getting the Languages...");
                        List<LanguageData> _languages = CRMOperations.GetLanguages(Service);
                        worker.ReportProgress(-1, "Getting the publishers...");
                        List<Publisher> _publishers = CRMOperations.GetPublishers(Service);
                        worker.ReportProgress(-1, "Getting the entities...");
                        int _alterantivelanguages = 3082;
                        if (mySettings.Lcid.HasValue)
                            _alterantivelanguages = mySettings.Lcid.Value;
                        List<EntityData> _entities = CRMOperations.GetEntities(Service, 1033, _alterantivelanguages);
                        worker.ReportProgress(-1, "Getting the Custom APIs...");
                        List<CustomAPI> _apis = CRMOperations.GetCustomAPIs(Service);
                        _response.Message = "The initial load was successful.";
                        _response.State = true;
                        _response.Publishers = _publishers;
                        _response.Entities = _entities;
                        _response.Apis = _apis;
                        _response.Languages = _languages;
                        worker.ReportProgress(-1, "Loading controls...");
                        eventWorker.Result = _response;
                    }
                    catch (Exception _ex)
                    {
                        LogError("An error occurred while connecting:" + _ex.Message + " - " + _ex.StackTrace);
                        _response.Message = "An error occurred while connecting";
                        _response.State = false;
                        _response.Exception = _ex;
                        eventWorker.Result = _response;
                    }
                },
                ProgressChanged = eventWorker =>
                {
                    SetWorkingMessage(eventWorker.UserState.ToString());
                },
                PostWorkCallBack = eventWorker =>
                {
                    ResponseCRMOperations _response = (ResponseCRMOperations)eventWorker.Result;
                    if (_response.State)
                    {
                        EnableAllControls(this);
                        LoadPublishers(_response.Publishers);
                        LoadEntities(_response.Entities);
                        LoadCustomAPIs(_response.Apis);
                        LoadLanguages(_response.Languages);
                        BtnExportSolution.Enabled = false;
                        CheckImportSolution.Checked = false;
                        CheckCopySolution.Checked = false;
                        CmbSolutionType.SelectedItem = null;
                        TxtCopySolution.Enabled = false;
                        BtnCopySolution.Enabled = false;
                        TxtCopySolution.Text = "";
                    }
                    else
                        ShowErrorDialog(_response.Exception, "Error", _response.Message, true);
                },
                AsyncArgument = null
            });
        }

        /// <summary>
        /// This event occurs when the control load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();
                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
                LogInfo("Settings found and loaded");
            LoadData();
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
        }

        /// <summary>
        /// This event occurs when the comboBox of message type changes value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListMessageType_SelectedValueChanged(object sender, EventArgs e)
        {
            if (ListMessageType.SelectedItem != null)
            {
                MessageType _seleccionado = (MessageType)ListMessageType.SelectedItem;
                ControlsPartsDisabled();
                CmbPartMessageEntity.SelectedItem = null;
                CmbPartMessageView.SelectedItem = null;

                if (_seleccionado.Id == 192350005)
                {
                    TxtPartMessageName.Enabled = true;
                    CheckPartMessageRequired.Enabled = true;
                    BtnAddPartMessage.Enabled = true;
                    CmbPartMessageEntity.Enabled = true;
                    CmbPartMessageView.Enabled = true;
                    CheckPartMessageALT.Enabled = true;
                    CheckPartMessageENG.Enabled = true;
                    TxtPartMessageDisplayName.Enabled = true;
                    TxtPartMessageDescription.Enabled = true;
                    NumMaxLength.Enabled = true;
                }
                else
                {
                    TxtPartMessageName.Enabled = true;
                    CheckPartMessageRequired.Enabled = true;
                    BtnAddPartMessage.Enabled = true;
                    CheckPartMessageALT.Enabled = true;
                    CheckPartMessageENG.Enabled = true;
                    TxtPartMessageDisplayName.Enabled = true;
                    TxtPartMessageDescription.Enabled = true;
                    NumMaxLength.Enabled = true;
                }
            }
        }

        /// <summary>
        /// This event occurs when the alternative language checkbox changes value (Message parts)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckPartMessageSPA_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckPartMessageALT.Checked)
            {
                TxtPartDisplayNameALT.Enabled = true;
                TxtPartDescriptionALT.Enabled = true;
            }
            else
            {
                TxtPartDisplayNameALT.Enabled = false;
                TxtPartDescriptionALT.Enabled = false;
            }
        }

        /// <summary>
        /// This event occurs when the english language checkbox changes value(Message parts)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckPartMessageENG_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckPartMessageENG.Checked)
            {
                TxtPartDisplayNameENG.Enabled = true;
                TxtPartDescriptionENG.Enabled = true;
            }
            else
            {
                TxtPartDisplayNameENG.Enabled = false;
                TxtPartDescriptionENG.Enabled = false;
            }
        }

        /// <summary>
        ///  This event occurs when the alternative language checkbox changes value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckSPA_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckALT.Checked)
            {
                TxtDisplayNameALT.Enabled = true;
                TxtDescriptionALT.Enabled = true;
                TxtSpecialConsentALT.Enabled = true;
            }
            else
            {
                TxtDisplayNameALT.Enabled = false;
                TxtDescriptionALT.Enabled = false;
                TxtSpecialConsentALT.Enabled = false;
            }
        }

        /// <summary>
        /// This event occurs when the english language checkbox changes value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckENG_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckENG.Checked)
            {
                TxtDisplayNameENG.Enabled = true;
                TxtDescriptionENG.Enabled = true;
                TxtSpecialConsentENG.Enabled = true;
            }
            else
            {
                TxtDisplayNameENG.Enabled = false;
                TxtDescriptionENG.Enabled = false;
                TxtSpecialConsentENG.Enabled = false;
            }

        }


        /// <summary>
        /// This event occurs when the message editor checkbox changes value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckMessageEditor_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckMessageEditor.Checked)
            {
                CmbEditorEntity.Enabled = true;
                CmbEditorForm.Enabled = false;
            }
            else
            {
                CmbEditorEntity.Enabled = false;
                CmbEditorForm.Enabled = false;

                CmbEditorEntity.SelectedItem = null;
                CmbEditorForm.SelectedItem = null;
            }
        }

        /// <summary>
        /// This event occurs when the Use existing solution checkbox changes value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckSolutionExists_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckSolutionExists.Checked)
            {
                CmbSolutionName.Visible = true;
                TxtSolutionName.Visible = false;
                CmbPublisher.SelectedItem = null;
                CmbSolutionName.Enabled = false;
            }
            else
            {
                CmbSolutionName.Visible = false;
                TxtSolutionName.Visible = true;
                CmbPublisher.SelectedItem = null;
            }
        }

        /// <summary>
        /// This event occurs when the Reload data button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLoading_Click(object sender, EventArgs e)
        {
            NuevaConfiguracion();
            LoadData();
        }

        /// <summary>
        /// Load the list of forms
        /// </summary>
        /// <param name="list">List of forms</param>
        private void LoadForms(List<FormData> list)
        {
            CmbConfigForm.Items.Clear();
            foreach (FormData _item in list)
            {
                CmbConfigForm.Items.Add(_item);
            }
        }

        /// <summary>
        /// Load the list of editor forms
        /// </summary>
        /// <param name="list">List of editor forms</param>
        private void LoadFormsEditor(List<FormData> list)
        {
            CmbEditorForm.Items.Clear();
            foreach (FormData _item in list)
            {
                CmbEditorForm.Items.Add(_item);
            }
        }

        /// <summary>
        /// Load the list of views
        /// </summary>
        /// <param name="list">List of views</param>
        private void LoadViews(List<ViewData> list)
        {
            CmbPartMessageView.Items.Clear();
            foreach (ViewData _item in list)
            {
                CmbPartMessageView.Items.Add(_item);
            }
        }


        /// <summary>
        /// Load the list of solutions
        /// </summary>
        /// <param name="list">List of solutions</param>
        private void LoadSolutions(List<SolutionData> list)
        {
            CmbSolutionName.Items.Clear();
            foreach (SolutionData _item in list)
            {
                CmbSolutionName.Items.Add(_item);
            }
        }


        /// <summary>
        /// This event occurs when the configuration form ComboBox changes value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbConfigEntity_SelectedValueChanged(object sender, EventArgs e)
        {
            if (_processComboChanges)
            {
                if (CmbConfigEntity.SelectedItem != null)
                {
                    EntityData _selected = (EntityData)CmbConfigEntity.SelectedItem;
                    WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Loading forms...",
                        Work = (worker, eventWorker) =>
                        {
                            ResponseOperations _response = new ResponseOperations();
                            try
                            {
                                List<FormData> _forms = CRMOperations.GetForms(Service, (int)eventWorker.Argument);

                                _response.Message = "The forms have been listed correctly.";
                                _response.State = true;
                                _response.Data = _forms;
                                eventWorker.Result = _response;
                            }
                            catch (Exception _ex)
                            {
                                LogError("This has occurred when performing the Form Search:" + _ex.Message + " - " + _ex.StackTrace);
                                _response.Message = "This has occurred when performing the Form Search";
                                _response.State = false;
                                _response.Exception = _ex;
                                eventWorker.Result = _response;
                            }
                        },
                        ProgressChanged = eventWorker =>
                        {
                            SetWorkingMessage(eventWorker.UserState.ToString());
                        },
                        PostWorkCallBack = eventWorker =>
                        {
                            ResponseOperations _response = (ResponseOperations)eventWorker.Result;
                            if (_response.State)
                            {
                                List<FormData> _forms = (List<FormData>)_response.Data;
                                if (_forms.Count > 0)
                                {
                                    LoadForms(_forms);
                                    CmbConfigForm.Enabled = true;
                                }
                                else
                                {
                                    CmbConfigForm.Enabled = false;
                                    MessageBox.Show("No forms have been found in this entity", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                            else
                            {
                                CmbConfigForm.Enabled = false;
                                ShowErrorDialog(_response.Exception, "Error", _response.Message, true);
                            }
                        },
                        AsyncArgument = _selected.Code
                    });
                }
            }
        }

        /// <summary>
        /// This event occurs when the Message Part Entity ComboBox changes value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbPartMessageEntity_SelectedValueChanged(object sender, EventArgs e)
        {
            if (_processComboChanges)
            {
                if (CmbPartMessageEntity.SelectedItem != null)
                {
                    EntityData _selected = (EntityData)CmbPartMessageEntity.SelectedItem;
                    WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Loading views...",
                        Work = (worker, eventWorker) =>
                        {
                            ResponseOperations _response = new ResponseOperations();
                            try
                            {
                                List<ViewData> _views = CRMOperations.GetViews(Service, (int)eventWorker.Argument);

                                _response.Message = "The views have been listed correctly.";
                                _response.State = true;
                                _response.Data = _views;
                                eventWorker.Result = _response;
                            }
                            catch (Exception _ex)
                            {
                                LogError("This has occurred when performing the View Search:" + _ex.Message + " - " + _ex.StackTrace);
                                _response.Message = "This has occurred when performing the View Search";
                                _response.State = false;
                                _response.Exception = _ex;
                                eventWorker.Result = _response;
                            }
                        },
                        ProgressChanged = eventWorker =>
                        {
                            SetWorkingMessage(eventWorker.UserState.ToString());
                        },
                        PostWorkCallBack = eventWorker =>
                        {
                            ResponseOperations _response = (ResponseOperations)eventWorker.Result;
                            if (_response.State)
                            {
                                List<ViewData> _views = (List<ViewData>)_response.Data;
                                if (_views.Count > 0)
                                {
                                    LoadViews(_views);
                                    CmbPartMessageView.Enabled = true;
                                }
                                else
                                {
                                    CmbPartMessageView.Enabled = false;
                                    MessageBox.Show("No views found for this entity", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                            else
                            {
                                CmbPartMessageView.Enabled = false;
                                ShowErrorDialog(_response.Exception, "Error", _response.Message, true);
                            }
                        },
                        AsyncArgument = _selected.Code
                    });
                }
            }
        }

        /// <summary>
        /// This event occurs when the additional entity ComboBox changes value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbEntityAdditional_SelectedValueChanged(object sender, EventArgs e)
        {
            if (_processComboChanges)
            {
                if (CmbEntityAdditional.SelectedItem != null)
                {
                    CheckAllComponent.Enabled = true;
                    BtnEntityAdditional.Enabled = true;
                }
                else
                {
                    CheckAllComponent.Enabled = false;
                    BtnEntityAdditional.Enabled = false;
                }
            }
        }

        /// <summary>
        /// This event occurs when the editor entity ComboBox changes value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbEditorEntity_SelectedValueChanged(object sender, EventArgs e)
        {
            if (_processComboChanges)
            {
                if (CmbEditorEntity.SelectedItem != null)
                {
                    EntityData _selected = (EntityData)CmbEditorEntity.SelectedItem;
                    WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Loading forms...",
                        Work = (worker, eventWorker) =>
                        {
                            ResponseOperations _response = new ResponseOperations();
                            try
                            {
                                List<FormData> _forms = CRMOperations.GetForms(Service, (int)eventWorker.Argument);

                                _response.Message = "The forms have been listed correctly.";
                                _response.State = true;
                                _response.Data = _forms;
                                eventWorker.Result = _response;
                            }
                            catch (Exception _ex)
                            {
                                LogError("This has occurred when performing the Form Search:" + _ex.Message + " - " + _ex.StackTrace);
                                _response.Message = "This has occurred when performing the Form Search";
                                _response.State = false;
                                _response.Exception = _ex;
                                eventWorker.Result = _response;
                            }
                        },
                        ProgressChanged = eventWorker =>
                        {
                            SetWorkingMessage(eventWorker.UserState.ToString());
                        },
                        PostWorkCallBack = eventWorker =>
                        {
                            ResponseOperations _response = (ResponseOperations)eventWorker.Result;
                            if (_response.State)
                            {
                                List<FormData> _forms = (List<FormData>)_response.Data;
                                if (_forms.Count > 0)
                                {
                                    LoadFormsEditor(_forms);
                                    CmbEditorForm.Enabled = true;
                                }
                                else
                                {
                                    CmbEditorForm.Enabled = false;
                                    MessageBox.Show("No forms have been found in this entity", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                            else
                            {
                                CmbConfigForm.Enabled = false;
                                ShowErrorDialog(_response.Exception, "Error", _response.Message, true);
                            }
                        },
                        AsyncArgument = _selected.Code
                    });

                }
            }
        }

        /// <summary>
        /// This event occurs when the add message part button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddPartMessage_Click(object sender, EventArgs e)
        {
            if (ListMessageType.SelectedItem != null)
            {
                MessageType _selected = (MessageType)ListMessageType.SelectedItem;

                if (CheckPartMessageALT.Checked && (string.IsNullOrEmpty(TxtPartDisplayNameALT.Text) || string.IsNullOrEmpty(TxtPartDescriptionALT.Text)))
                    MessageBox.Show("If you mark the labels in Spanish, you must add the display name and description", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (CheckPartMessageENG.Checked && (string.IsNullOrEmpty(TxtPartDisplayNameENG.Text) || string.IsNullOrEmpty(TxtPartDescriptionENG.Text)))
                    MessageBox.Show("If you mark the labels in English, you must add the display name and description", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    if (_selected.Id == 192350005)
                    {
                        if (!string.IsNullOrEmpty(TxtPartMessageName.Text))
                        {
                            if (GeneralOperations.IsValidName(TxtPartMessageName.Text))
                            {
                                if (!string.IsNullOrEmpty(TxtPartMessageDisplayName.Text))
                                {
                                    if (!string.IsNullOrEmpty(TxtPartMessageDescription.Text))
                                    {
                                        if (CmbPartMessageEntity.SelectedItem != null)
                                        {
                                            if (CmbPartMessageView.SelectedItem != null)
                                            {
                                                string[] _row = { Guid.NewGuid().ToString(), TxtPartMessageName.Text, TxtPartMessageDisplayName.Text, TxtPartMessageDescription.Text, NumMaxLength.Value.ToString(), _selected.Id.ToString(), CheckPartMessageRequired.Checked.ToString(), ((EntityData)CmbPartMessageEntity.SelectedItem).DisplayName, ((EntityData)CmbPartMessageEntity.SelectedItem).Code.ToString(), ((EntityData)CmbPartMessageEntity.SelectedItem).LogicalName, ((ViewData)CmbPartMessageView.SelectedItem).Name.ToString(), ((ViewData)CmbPartMessageView.SelectedItem).Id.ToString(), CheckPartMessageALT.Checked.ToString(), CheckPartMessageENG.Checked.ToString(), TxtPartDisplayNameALT.Text, TxtPartDisplayNameENG.Text, TxtPartDescriptionALT.Text, TxtPartDescriptionENG.Text };
                                                ListMessagesParts.Items.Add(new ListViewItem(_row));
                                                TxtPartMessageName.Text = "";
                                                CmbPartMessageEntity.SelectedItem = null;
                                                CmbPartMessageView.SelectedItem = null;
                                                CheckPartMessageRequired.Checked = false;
                                                CheckPartMessageALT.Checked = false;
                                                CheckPartMessageENG.Checked = false;
                                                ListMessageType.SelectedItem = null;
                                                TxtPartDisplayNameALT.Text = "";
                                                TxtPartDescriptionALT.Text = "";
                                                TxtPartDisplayNameENG.Text = "";
                                                TxtPartDescriptionENG.Text = "";
                                                TxtPartMessageDisplayName.Text = "";
                                                TxtPartMessageDescription.Text = "";
                                                NumMaxLength.Value = 100;
                                                ControlsPartsDisabled();

                                            }
                                            else
                                                MessageBox.Show("You must set a view of the message part, to continue", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        }
                                        else
                                            MessageBox.Show("You must set a message part entity to continue.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else
                                        MessageBox.Show("You must set a description of the message part to continue.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else
                                    MessageBox.Show("You must set a display name for the message part to continue.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                                MessageBox.Show("The name assigned to the message part is invalid. It:\nMust not contain spaces\nMust not contain special characters\nOnly underscores (_) and hyphens (-) are allowed", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                            MessageBox.Show("You must choose a message type to continue.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(TxtPartMessageName.Text))
                        {
                            if (GeneralOperations.IsValidName(TxtPartMessageName.Text))
                            {
                                if (!string.IsNullOrEmpty(TxtPartMessageDisplayName.Text))
                                {
                                    if (!string.IsNullOrEmpty(TxtPartMessageDescription.Text))
                                    {
                                        string[] _row = { Guid.NewGuid().ToString(), TxtPartMessageName.Text, TxtPartMessageDisplayName.Text, TxtPartMessageDescription.Text, NumMaxLength.Value.ToString(), _selected.Id.ToString(), CheckPartMessageRequired.Checked.ToString(), "", "", "", "", "", CheckPartMessageALT.Checked.ToString(), CheckPartMessageENG.Checked.ToString(), TxtPartDisplayNameALT.Text, TxtPartDisplayNameENG.Text, TxtPartDescriptionALT.Text, TxtPartDescriptionENG.Text };
                                        ListMessagesParts.Items.Add(new ListViewItem(_row));
                                        TxtPartMessageName.Text = "";
                                        CmbPartMessageEntity.SelectedItem = null;
                                        CmbPartMessageView.SelectedItem = null;
                                        CheckPartMessageRequired.Checked = false;
                                        CheckPartMessageALT.Checked = false;
                                        CheckPartMessageENG.Checked = false;
                                        ListMessageType.SelectedItem = null;
                                        TxtPartDisplayNameALT.Text = "";
                                        TxtPartDescriptionALT.Text = "";
                                        TxtPartDisplayNameENG.Text = "";
                                        TxtPartDescriptionENG.Text = "";
                                        TxtPartMessageDisplayName.Text = "";
                                        TxtPartMessageDescription.Text = "";
                                        NumMaxLength.Value = 100;
                                        ControlsPartsDisabled();
                                    }
                                    else
                                        MessageBox.Show("You must set a description of the message part to continue.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else
                                    MessageBox.Show("You must set a display name for the message part to continue.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                                MessageBox.Show("The name assigned to the message part is invalid. It:\nMust not contain spaces\nMust not contain special characters\nOnly underscores (_) and hyphens (-) are allowed", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                            MessageBox.Show("You must set a message part name to continue.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
                MessageBox.Show("You must choose a message type to continue.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        /// <summary>
        /// This event occurs when the publishers comboBox changes value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbPublisher_SelectedValueChanged(object sender, EventArgs e)
        {
            if (_processComboChanges)
            {
                if (CheckSolutionExists.Checked)
                {
                    if (CmbPublisher.SelectedItem != null)
                    {
                        Publisher _selected = (Publisher)CmbPublisher.SelectedItem;
                        WorkAsync(new WorkAsyncInfo
                        {
                            Message = "Loading solutions...",
                            Work = (worker, eventWorker) =>
                            {
                                ResponseOperations _response = new ResponseOperations();
                                try
                                {
                                    List<SolutionData> _solutions = CRMOperations.GetSolutions(Service, (Guid)eventWorker.Argument);

                                    _response.Message = "The solutions have been listed correctly.";
                                    _response.State = true;
                                    _response.Data = _solutions;
                                    eventWorker.Result = _response;
                                }
                                catch (Exception _ex)
                                {
                                    LogError("This has occurred when performing the Solutions Search:" + _ex.Message + " - " + _ex.StackTrace);
                                    _response.Message = "This has occurred when performing the Solutions Search";
                                    _response.State = false;
                                    _response.Exception = _ex;
                                    eventWorker.Result = _response;
                                }
                            },
                            ProgressChanged = eventWorker =>
                            {
                                SetWorkingMessage(eventWorker.UserState.ToString());
                            },
                            PostWorkCallBack = eventWorker =>
                            {
                                ResponseOperations _response = (ResponseOperations)eventWorker.Result;
                                if (_response.State)
                                {
                                    List<SolutionData> _soluciones = (List<SolutionData>)_response.Data;
                                    if (_soluciones.Count > 0)
                                    {
                                        LoadSolutions(_soluciones);
                                        CmbSolutionName.Enabled = true;
                                    }
                                    else
                                    {
                                        CmbSolutionName.Enabled = false;
                                        MessageBox.Show("No solutions have been found in this publisher", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                                else
                                {
                                    CmbSolutionName.Enabled = false;
                                    ShowErrorDialog(_response.Exception, "Error", _response.Message, true);
                                }
                            },
                            AsyncArgument = _selected.Id
                        });
                    }
                }
            }
        }

        /// <summary>
        /// This event occurs when the delete message part button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDeletePartMessage_Click(object sender, EventArgs e)
        {
            if (ListMessagesParts.SelectedIndices.Count > 0)
            {
                DialogResult _confirmation = MessageBox.Show("Do you want to delete the selected row?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (_confirmation == DialogResult.Yes)
                    ListMessagesParts.Items.RemoveAt(ListMessagesParts.SelectedIndices[0]);
            }
            else
                MessageBox.Show("Please select an item to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Verifies that the specified component has already been added to the list
        /// </summary>
        /// <param name="searchedValue">Value to search</param>
        /// <param name="columnNumber">Number of the column to search</param>
        /// <returns>True if it exists or False if it does not.</returns>
        private bool ExistsInColumnComponents(string searchedValue, int columnNumber)
        {
            foreach (ListViewItem _item in ListEntitiesAdd.Items)
            {
                if (_item.SubItems[columnNumber].Text.Equals(searchedValue, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// This event occurs when the add entity additional button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEntityAdditional_Click(object sender, EventArgs e)
        {
            if (CmbEntityAdditional.SelectedItem != null)
            {
                EntityData _selected = (EntityData)CmbEntityAdditional.SelectedItem;
                if (!ExistsInColumnComponents(_selected.Code.ToString(), 0))
                {
                    string[] _row = { _selected.Code.ToString(), _selected.DisplayName, _selected.LogicalName, CheckAllComponent.Checked.ToString() };
                    ListEntitiesAdd.Items.Add(new ListViewItem(_row));
                    CmbEntityAdditional.SelectedItem = null;
                    CheckAllComponent.Checked = false;
                    CheckAllComponent.Enabled = false;
                    BtnEntityAdditional.Enabled = false;
                }
                else
                    MessageBox.Show("The specified component has already been added. Please try another one.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                MessageBox.Show("Please select an entity from the list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// This event occurs when the delete entity additional button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteComponent_Click(object sender, EventArgs e)
        {
            if (ListEntitiesAdd.SelectedIndices.Count > 0)
            {
                DialogResult _confirmation = MessageBox.Show("Do you want to delete the selected row?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (_confirmation == DialogResult.Yes)
                    ListEntitiesAdd.Items.RemoveAt(ListEntitiesAdd.SelectedIndices[0]);
            }
            else
                MessageBox.Show("Please select an item to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        ///  This event occurs when the export solution button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExportSolution_Click(object sender, EventArgs e)
        {
            if (_generatedSolution != null)
            {
                if (CmbSolutionType.SelectedItem != null)
                {
                    DataFileSolution _solutionData = new DataFileSolution()
                    {
                        Managed = CmbSolutionType.SelectedText.Equals("Managed")
                    };
                    using (SaveFileDialog _saveDialog = new SaveFileDialog())
                    {
                        _saveDialog.FileName = (_generatedSolution != null) ? (_generatedSolution + ".zip") : "Solucion.zip";
                        _saveDialog.Filter = "ZIP Compressed (*.zip)|*.zip|All files (*.*)|*.*";
                        _saveDialog.Title = "Save Solution";


                        if (_saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            _solutionData.Path = _saveDialog.FileName;
                            WorkAsync(new WorkAsyncInfo
                            {
                                Message = "Exporting solution...",
                                Work = (worker, eventWorker) =>
                                {
                                    ResponseOperations _response = new ResponseOperations();
                                    try
                                    {
                                        DataFileSolution _data = (DataFileSolution)eventWorker.Argument;
                                        CRMOperations.ExportSolution(Service, _generatedSolution, _data.Path, true, _data.Managed);

                                        _response.Message = "The solution has been exported successfully.";
                                        _response.State = true;
                                        eventWorker.Result = _response;
                                    }
                                    catch (Exception _ex)
                                    {
                                        LogError("It occurred when exporting the solution: " + _ex.Message + " - " + _ex.StackTrace);
                                        _response.Message = "It occurred when exporting the solution.";
                                        _response.State = false;
                                        _response.Exception = _ex;
                                        eventWorker.Result = _response;
                                    }
                                },
                                ProgressChanged = eventWorker =>
                                {
                                    SetWorkingMessage(eventWorker.UserState.ToString());
                                },
                                PostWorkCallBack = eventWorker =>
                                {
                                    ResponseOperations _response = (ResponseOperations)eventWorker.Result;
                                    if (_response.State)
                                        MessageBox.Show(_response.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    else
                                        MessageBox.Show(_response.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                },
                                AsyncArgument = _solutionData
                            });
                        }
                    }
                }
                else
                    MessageBox.Show("You must select a solution type before exporting", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                MessageBox.Show("No solution has been generated", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Load new configuraion 
        /// </summary>
        private void NuevaConfiguracion()
        {
            _processComboChanges = false;
            CmbConfigEntity.Focus();
            CmbConfigEntity.SelectedText = "";
            CmbConfigForm.Focus();
            CmbConfigForm.SelectedText = "";
            CmbConfigForm.Enabled = false;
            CheckALT.Checked = false;
            CheckENG.Checked = false;
            TxtDisplayNameALT.Text = "";
            TxtDescriptionALT.Text = "";
            TxtSpecialConsentALT.Text = "";
            TxtDisplayNameENG.Text = "";
            TxtDescriptionENG.Text = "";
            TxtSpecialConsentENG.Text = "";

            TxtPartMessageName.Text = "";
            TxtPartMessageDisplayName.Text = "";
            TxtPartMessageDescription.Text = "";

            CmbPartMessageEntity.Focus();
            CmbPartMessageEntity.SelectedText = "";
            CmbPartMessageView.Focus();
            CmbPartMessageView.SelectedText = "";
            CheckPartMessageRequired.Checked = false;
            CheckPartMessageALT.Checked = false;
            CheckPartMessageENG.Checked = false;

            ListMessageType.SelectedItem = null;
            TxtPartDisplayNameALT.Text = "";
            TxtPartDescriptionALT.Text = "";
            TxtPartDisplayNameENG.Text = "";
            TxtPartDescriptionENG.Text = "";
            ControlsPartsDisabled();

            ListMessagesParts.Items.Clear();
            BtnExportSolution.Enabled = false;

            CmbEditorEntity.Focus();
            CmbEditorEntity.SelectedText = "";
            CmbEditorForm.Focus();
            CmbEditorForm.SelectedText = "";
            CmbEditorForm.Enabled = false;
            CheckMessageEditor.Checked = false;

            CmbPublisher.Focus();
            CmbPublisher.SelectedText = "";
            CmbSolutionName.Focus();
            CmbSolutionName.SelectedText = "";
            CheckSolutionExists.Checked = false;
            TxtSolutionName.Text = "";
            TxtChannelName.Text = "";
            CmbCustomAPI.Focus();
            CmbCustomAPI.SelectedText = "";
            TxtChannelDescription.Text = "";
            CheckInbound.Checked = false;
            CheckDelivery.Checked = false;
            CheckAttachment.Checked = false;
            CheckBinary.Checked = false;
            CheckSpecialConsent.Checked = false;
            CmbConfigEntity.Focus();

            CmbEntityAdditional.Focus();
            CmbEntityAdditional.SelectedText = "";

            CheckAllComponent.Checked = false;
            CheckAllComponent.Enabled = false;
            BtnEntityAdditional.Enabled = false;
            ListEntitiesAdd.Items.Clear();

            _generatedChannelId = Guid.NewGuid();
            _generatedLocaleIdALT = Guid.NewGuid();
            _generatedLocaleIdENG = Guid.NewGuid();
            _processComboChanges = true;
        }

        /// <summary>
        /// This event occurs when the new configuration button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult _confirmation = MessageBox.Show("Do you want to set a new configuration?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (_confirmation == DialogResult.Yes)
                NuevaConfiguracion();
        }

        /// <summary>
        /// The information of the new channel is obtained
        /// </summary>
        /// <returns>Channel Information</returns>
        private ChannelData GetInfoChannel()
        {
            ChannelData _channel = new ChannelData
            {
                ChannelId = _generatedChannelId,
                LocaleIdALT = _generatedLocaleIdALT,
                LocaleIdENG = _generatedLocaleIdENG,
                ConfigurationEntity = (EntityData)CmbConfigEntity.SelectedItem,
                ConfigurationForm = (FormData)CmbConfigForm.SelectedItem,
                ExistingSolution = CheckSolutionExists.Checked,
                Publisher = (Publisher)CmbPublisher.SelectedItem,
                ChannelName = TxtChannelName.Text,
                CustomAPI = (CustomAPI)CmbCustomAPI.SelectedItem,
                Description = TxtChannelDescription.Text,
                AllowAttachment = CheckAttachment.Checked,
                AllowBinary = CheckBinary.Checked,
                AllowInbound = CheckInbound.Checked,
                AllowDelivery = CheckDelivery.Checked,
                RequiresSpecialConsent = CheckSpecialConsent.Checked,
                PublishStart = false
            };

            List<MessagePartData> _messageParts = new List<MessagePartData>();
            int _indice = 1;
            foreach (ListViewItem _item in ListMessagesParts.Items)
            {

                MessagePartData _part = new MessagePartData()
                {
                    Id = new Guid(_item.SubItems[0].Text),
                    ChannelId = _channel.ChannelId,
                    Name = _item.SubItems[1].Text,
                    DisplayName = _item.SubItems[2].Text,
                    Description = _item.SubItems[3].Text,
                    MaxLength = Convert.ToInt32(_item.SubItems[4].Text),
                    Type = Convert.ToInt32(_item.SubItems[5].Text),
                    Required = Convert.ToBoolean(_item.SubItems[6].Text),
                    State = 0,
                    Status = 1,
                    Order = _indice
                };

                //para Lookup
                if (Convert.ToInt32(_item.SubItems[5].Text) == 192350005)
                {
                    _part.EntityName = _item.SubItems[7].Text;
                    _part.EntityId = Convert.ToInt32(_item.SubItems[8].Text);
                    _part.Entity = _item.SubItems[9].Text;
                    _part.ViewName = _item.SubItems[10].Text;
                    _part.ViewId = new Guid(_item.SubItems[11].Text);
                }

                //Etiquetas español
                if (Convert.ToBoolean(_item.SubItems[12].Text))
                {
                    _part.LabelNameALT = _item.SubItems[14].Text;
                    _part.LabelDescriptionALT = _item.SubItems[16].Text;
                }

                //Etiquetas ingles
                if (Convert.ToBoolean(_item.SubItems[13].Text))
                {
                    _part.LabelNameENG = _item.SubItems[15].Text;
                    _part.LabelDescriptionENG = _item.SubItems[17].Text;
                }
                _indice++;
                _messageParts.Add(_part);
            }
            _channel.MessagesParts = _messageParts;

            if (CheckSolutionExists.Checked)
            {
                _channel.Solution = (SolutionData)CmbSolutionName.SelectedItem;
                _channel.SolutionName = null;
            }
            else
            {
                _channel.Solution = null;
                _channel.SolutionName = TxtSolutionName.Text;
            }

            if (CheckMessageEditor.Checked)
            {
                _channel.EditorEntity = (EntityData)CmbEditorEntity.SelectedItem;
                _channel.EditorForm = (FormData)CmbEditorForm.SelectedItem;
            }
            else
            {
                _channel.EditorEntity = null;
                _channel.EditorForm = null;
            }

            if (CheckALT.Checked)
            {
                LabelData _label = new LabelData
                {
                    Name = TxtDisplayNameALT.Text,
                    Description = TxtDescriptionALT.Text,
                    SpecialConsent = TxtSpecialConsentALT.Text
                };
                _channel.LabelAlternative = _label;
            }
            else
                _channel.LabelAlternative = null;

            if (CheckENG.Checked)
            {
                LabelData _label = new LabelData
                {
                    Name = TxtDisplayNameENG.Text,
                    Description = TxtDescriptionENG.Text,
                    SpecialConsent = TxtSpecialConsentENG.Text
                };
                _channel.LabelEnglish = _label;
            }
            else
                _channel.LabelEnglish = null;

            List<AdditionalEntity> _additionalEntities = new List<AdditionalEntity>();
            foreach (ListViewItem _item in ListEntitiesAdd.Items)
            {
                AdditionalEntity _entityData = new AdditionalEntity()
                {
                    Code = Convert.ToInt32(_item.SubItems[0].Text),
                    DisplayName = _item.SubItems[1].Text,
                    LogicalName = _item.SubItems[2].Text,
                    AllComponent = Convert.ToBoolean(_item.SubItems[3].Text)
                };
                _additionalEntities.Add(_entityData);
            }
            _channel.AdditionalEntities = _additionalEntities;

            _channel.PublishStart = false;
            return _channel;
        }

        /// <summary>
        /// Loads a channel's information into the plugin controls
        /// </summary>
        /// <param name="channel">Channel information</param>
        private void LoadInfoChannelControls(ChannelData channel)
        {
            _processComboChanges = false;
            //Assign values ​​to user interface controls
            _generatedChannelId = channel.ChannelId;
            _generatedLocaleIdALT = channel.LocaleIdALT;
            _generatedLocaleIdENG = channel.LocaleIdENG;

            // ComboBox: Assign the selected values
            if (channel.ConfigurationEntity != null)
            {
                EntityData _configurationEntity = CmbConfigEntity.Items.Cast<EntityData>().FirstOrDefault(x => x.Code == channel.ConfigurationEntity.Code);
                CmbConfigEntity.SelectedItem = _configurationEntity;
            }

            if (channel.ConfigurationForm != null)
            {
                FormData _configurationForm = CmbConfigForm.Items.Cast<FormData>().FirstOrDefault(x => x.Id == channel.ConfigurationForm.Id);
                CmbConfigForm.Enabled = true;
                CmbConfigForm.SelectedItem = _configurationForm;
            }
            else
                CmbConfigForm.Enabled = false;

            CheckSolutionExists.Checked = channel.ExistingSolution;
            if (channel.Publisher != null)
            {
                Publisher _publisherInfo = CmbPublisher.Items.Cast<Publisher>().FirstOrDefault(x => x.Id == channel.Publisher.Id);
                CmbPublisher.SelectedItem = _publisherInfo;
            }

            TxtSolutionName.Text = channel.SolutionName;
            if (channel.CustomAPI != null)
            {
                CustomAPI _api = CmbCustomAPI.Items.Cast<CustomAPI>().FirstOrDefault(x => x.Id == channel.CustomAPI.Id);
                CmbCustomAPI.SelectedItem = _api;
            }
            TxtChannelName.Text = channel.ChannelName;
            TxtChannelDescription.Text = channel.Description;
            CheckAttachment.Checked = channel.AllowAttachment;
            CheckBinary.Checked = channel.AllowBinary;
            CheckInbound.Checked = channel.AllowInbound;
            CheckDelivery.Checked = channel.AllowDelivery;
            CheckSpecialConsent.Checked = channel.RequiresSpecialConsent;

            //Assign message parts in the ListView
            ListMessagesParts.Items.Clear();
            if (channel.MessagesParts != null && channel.MessagesParts.Count > 0)
            {
                foreach (MessagePartData _part in channel.MessagesParts)
                {
                    ListViewItem _item = new ListViewItem(_part.Id.ToString());
                    _item.SubItems.Add(_part.Name);
                    _item.SubItems.Add(_part.DisplayName);
                    _item.SubItems.Add(_part.Description);
                    _item.SubItems.Add(_part.MaxLength.ToString());
                    _item.SubItems.Add(_part.Type.ToString());
                    _item.SubItems.Add(_part.Required.ToString());

                    // Lookup
                    if (_part.Type == 192350005)
                    {
                        _item.SubItems.Add(_part.EntityName);
                        _item.SubItems.Add(_part.EntityId.ToString());
                        _item.SubItems.Add(_part.Entity);
                        _item.SubItems.Add(_part.ViewName);
                        _item.SubItems.Add(_part.ViewId.ToString());
                    }
                    else
                    {
                        _item.SubItems.Add("");
                        _item.SubItems.Add("");
                        _item.SubItems.Add("");
                        _item.SubItems.Add("");
                        _item.SubItems.Add("");
                    }

                    // Etiquetas en español
                    if (_part.LabelNameALT != null && _part.LabelDescriptionALT != null)
                        _item.SubItems.Add("True");
                    else
                        _item.SubItems.Add("False");

                    if (_part.LabelNameENG != null && _part.LabelDescriptionENG != null)
                        _item.SubItems.Add("True");
                    else
                        _item.SubItems.Add("False");

                    _item.SubItems.Add(_part.LabelNameALT ?? "");
                    _item.SubItems.Add(_part.LabelNameENG ?? "");
                    _item.SubItems.Add(_part.LabelDescriptionALT ?? "");
                    _item.SubItems.Add(_part.LabelDescriptionENG ?? "");

                    ListMessagesParts.Items.Add(_item);
                }
            }

            // Assign the solution
            if (channel.ExistingSolution)
            {
                CheckSolutionExists.Checked = true;
                if (channel.Solution != null)
                {
                    SolutionData _solutionInfo = CmbSolutionName.Items.Cast<SolutionData>().FirstOrDefault(x => x.Id == channel.Solution.Id);
                    CmbSolutionName.SelectedItem = _solutionInfo;
                }

                TxtSolutionName.Clear();
                if (channel.Publisher != null)
                    CmbSolutionName.Enabled = true;
                else
                    CmbSolutionName.Enabled = false;
            }
            else
            {
                CheckSolutionExists.Checked = false;
                TxtSolutionName.Text = channel.SolutionName ?? "";
                CmbSolutionName.SelectedItem = null;
            }

            // Assign editor
            if (channel.EditorEntity != null)
            {
                CheckMessageEditor.Checked = true;

                EntityData _editorEntity = CmbEditorEntity.Items.Cast<EntityData>().FirstOrDefault(x => x.Code == channel.EditorEntity.Code);
                CmbEditorEntity.SelectedItem = _editorEntity;

                if (channel.EditorForm != null)
                {
                    FormData _editorForm = CmbEditorForm.Items.Cast<FormData>().FirstOrDefault(x => x.Id == channel.EditorForm.Id);
                    CmbEditorForm.SelectedItem = _editorForm;
                    CmbEditorForm.Enabled = true;
                }
                else
                    CmbEditorForm.Enabled = false;
            }
            else
            {
                CmbEditorEntity.SelectedItem = null;
                CmbEditorForm.SelectedItem = null;
                CmbEditorForm.Enabled = false;
                CheckMessageEditor.Checked = false;
            }

            // Assign labels
            if (channel.LabelAlternative != null)
            {
                CheckALT.Checked = true;
                TxtDisplayNameALT.Text = channel.LabelAlternative?.Name ?? "";
                TxtDescriptionALT.Text = channel.LabelAlternative?.Description ?? "";
                TxtSpecialConsentALT.Text = channel.LabelAlternative?.SpecialConsent ?? "";
            }
            else
            {
                CheckALT.Checked = false;
                TxtDisplayNameALT.Clear();
                TxtDescriptionALT.Clear();
                TxtSpecialConsentALT.Clear();
            }

            if (channel.LabelEnglish != null)
            {
                CheckENG.Checked = true;
                TxtDisplayNameENG.Text = channel.LabelEnglish?.Name ?? "";
                TxtDescriptionENG.Text = channel.LabelEnglish?.Description ?? "";
                TxtSpecialConsentENG.Text = channel.LabelEnglish?.SpecialConsent ?? "";
            }
            else
            {
                CheckENG.Checked = false;
                TxtDisplayNameENG.Clear();
                TxtDescriptionENG.Clear();
                TxtSpecialConsentENG.Clear();
            }

            ListEntitiesAdd.Items.Clear();
            if (channel.AdditionalEntities != null && channel.AdditionalEntities.Count > 0)
            {
                foreach (AdditionalEntity _additional in channel.AdditionalEntities)
                {
                    ListViewItem _item = new ListViewItem(_additional.Code.ToString());
                    _item.SubItems.Add(_additional.DisplayName);
                    _item.SubItems.Add(_additional.LogicalName);
                    _item.SubItems.Add(_additional.AllComponent.ToString());
                    ListEntitiesAdd.Items.Add(_item);
                }
            }
            _processComboChanges = true;
        }

        /// <summary>
        /// This event occurs when the button to open Json file is clicked 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openJsonFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog _openDialog = new OpenFileDialog())
            {
                _openDialog.Filter = "JavaScript Object Notation (*.json)|*.json|All Files (*.*)|*.*";
                _openDialog.Title = "Open Settings";

                if (_openDialog.ShowDialog() == DialogResult.OK)
                {
                    WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Opening file...",
                        Work = (worker, eventWorker) =>
                        {
                            ResponseOperationsRead _response = new ResponseOperationsRead();
                            try
                            {
                                ChannelData _channel = FileOperations.ReadObjectFromJson(eventWorker.Argument.ToString());
                                _response.Channel = _channel;

                                //Configuration Forms
                                if (_channel.ConfigurationEntity != null)
                                {
                                    List<FormData> _listConfigurationForms = CRMOperations.GetForms(Service, _channel.ConfigurationEntity.Code);
                                    _response.ConfigurationForms = _listConfigurationForms;
                                }
                                else
                                    _response.ConfigurationForms = new List<FormData>();

                                //Editor Forms
                                if (_channel.EditorEntity != null)
                                {
                                    List<FormData> _listEditorForms = CRMOperations.GetForms(Service, _channel.EditorEntity.Code);
                                    _response.EditorForms = _listEditorForms;
                                }
                                else
                                    _response.EditorForms = new List<FormData>();

                                //Existing solutions
                                if (_channel.ExistingSolution && _channel.Publisher != null)
                                {
                                    List<SolutionData> _listSolutions = CRMOperations.GetSolutions(Service, _channel.Publisher.Id);
                                    _response.Solutions = _listSolutions;
                                }
                                else
                                    _response.Solutions = new List<SolutionData>();


                                _response.Message = "The reading of the configuration file has completed successfully.";
                                _response.State = true;
                                eventWorker.Result = _response;
                            }
                            catch (Exception _ex)
                            {
                                LogError("An error occurred while reading the configuration file: " + _ex.Message + " - " + _ex.StackTrace);
                                _response.Message = "An error occurred while reading the configuration file.";
                                _response.State = false;
                                _response.Exception = _ex;
                                eventWorker.Result = _response;
                            }
                        },
                        ProgressChanged = eventWorker =>
                        {
                            SetWorkingMessage(eventWorker.UserState.ToString());
                        },
                        PostWorkCallBack = eventWorker =>
                        {
                            ResponseOperationsRead _response = (ResponseOperationsRead)eventWorker.Result;
                            if (_response.State)
                            {
                                LoadForms(_response.ConfigurationForms);
                                LoadFormsEditor(_response.EditorForms);
                                LoadSolutions(_response.Solutions);

                                LoadInfoChannelControls(_response.Channel);
                                MessageBox.Show(_response.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                                MessageBox.Show(_response.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        },
                        AsyncArgument = _openDialog.FileName
                    });

                }
            }
        }

        /// <summary>
        /// This event occurs when the button to save Json file is clicked 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveJSONFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog _saveDialog = new SaveFileDialog())
            {
                _saveDialog.FileName = DateTime.UtcNow.Ticks.ToString() + ".json";
                _saveDialog.Filter = "JavaScript Object Notation (*.json)|*.json|All files (*.*)|*.*";
                _saveDialog.Title = "Export Settings";

                if (_saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ChannelData _channel = GetInfoChannel();
                    JsonExport _data = new JsonExport()
                    {
                        Channel = _channel,
                        Path = _saveDialog.FileName
                    };

                    WorkAsync(new WorkAsyncInfo
                    {
                        Message = "Saving file...",
                        Work = (worker, eventWorker) =>
                        {
                            ResponseOperations _response = new ResponseOperations();
                            try
                            {
                                JsonExport _dataFile = (JsonExport)eventWorker.Argument;
                                FileOperations.SaveObjectAsJson(_dataFile.Channel, _dataFile.Path);
                                _response.Message = "File saved successfully.";
                                _response.State = true;
                                eventWorker.Result = _response;
                            }
                            catch (Exception _ex)
                            {
                                LogError("An error occurred while saving the file.: " + _ex.Message + " - " + _ex.StackTrace);
                                _response.Message = "An error occurred while saving the file.";
                                _response.State = false;
                                _response.Exception = _ex;
                                eventWorker.Result = _response;
                            }
                        },
                        ProgressChanged = eventWorker =>
                        {
                            SetWorkingMessage(eventWorker.UserState.ToString());
                        },
                        PostWorkCallBack = eventWorker =>
                        {
                            ResponseOperations _response = (ResponseOperations)eventWorker.Result;
                            if (_response.State)
                                MessageBox.Show(_response.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            else
                                MessageBox.Show(_response.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        },
                        AsyncArgument = _data
                    });

                }
            }
        }

        /// <summary>
        /// This event occurs when the language combobox changes value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbLanguage.SelectedItem != null)
            {
                LanguageData _selected = (LanguageData)CmbLanguage.SelectedItem;
                CheckALT.Text = _selected.DisplayName;
                CheckPartMessageALT.Text = _selected.DisplayName;
                mySettings.Lcid = _selected.Lcid;
                mySettings.LanguageALTName = _selected.DisplayName;
            }
        }

        /// <summary>
        /// This event occurs when the plugin tab has been closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyCustomChannelControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the create solution button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCreateSolution_Click(object sender, EventArgs e)
        {

            if (CmbConfigEntity.SelectedItem == null || CmbConfigForm.SelectedItem == null)
                MessageBox.Show("You must select an entity and a configuration form", "Step #1 - Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (ListMessagesParts.Items.Count == 0)
                MessageBox.Show("You must select to add at least one (1) type of message parts", "Step #2 - Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (CheckMessageEditor.Checked && (CmbEditorEntity.SelectedItem == null || CmbEditorForm.SelectedItem == null))
                MessageBox.Show("You must select an entity and a form for the message editor.", "Step #4 - Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (CheckALT.Checked && (string.IsNullOrEmpty(TxtDisplayNameALT.Text) || string.IsNullOrEmpty(TxtDescriptionALT.Text) || string.IsNullOrEmpty(TxtSpecialConsentALT.Text)))
                MessageBox.Show("You must select to specify the name, description, and consent of the labels in " + CmbLanguage.SelectedText, "Step #3 - Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (CheckENG.Checked && (string.IsNullOrEmpty(TxtDisplayNameENG.Text) || string.IsNullOrEmpty(TxtDescriptionENG.Text) || string.IsNullOrEmpty(TxtSpecialConsentENG.Text)))
                MessageBox.Show("You must select to specify the name, description, and consent of the labels in English", "Step #3 - Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (CheckSolutionExists.Checked && (CmbPublisher.SelectedItem == null || CmbSolutionName.SelectedItem == null))
                MessageBox.Show("You must select a publisher and an existing solution.", "Step #5 - Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (!CheckSolutionExists.Checked && (CmbPublisher.SelectedItem == null || string.IsNullOrEmpty(TxtSolutionName.Text)))
                MessageBox.Show("You must select a publisher and a name for the solution.", "Step #5 - Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (string.IsNullOrEmpty(TxtChannelName.Text))
                MessageBox.Show("You must provide a name for the channel", "Step #5 - Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (CmbCustomAPI.SelectedItem == null)
                MessageBox.Show("You must select a custom API.", "Step #5 - Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (string.IsNullOrEmpty(TxtChannelDescription.Text))
                MessageBox.Show("You must provide a description for the channel", "Step #5 - Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (!CheckImportSolution.Checked || !CheckCopySolution.Checked)
                MessageBox.Show("To generate the channel, you must indicate whether the solution will be imported or copied. Both cannot be deselected.", "Review - Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (CheckCopySolution.Checked && TxtCopySolution.Text == "")
                MessageBox.Show("You must choose a folder where the copy of the solution will be stored.", "Review - Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                ChannelData _channelParameter = GetInfoChannel();
                //AdditionalDataProcess
                AdditionalDataProcess _dataProcess = new AdditionalDataProcess()
                {
                    ImporSolution = CheckImportSolution.Checked,
                    CopySolution = CheckCopySolution.Checked,
                    PathCopy = TxtCopySolution.Text
                };

                if (!CheckSolutionExists.Checked)
                {
                    DialogResult _confirmation = MessageBox.Show("Would you like to publish the generated solution?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (_confirmation == DialogResult.Yes)
                        _channelParameter.PublishStart = true;
                }

                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Starting the channel creation process...",
                    Work = (worker, eventWorker) =>
                    {
                        ResponseOperations _response = new ResponseOperations();
                        try
                        {
                            ChannelData _channel = (ChannelData)eventWorker.Argument;
                            SolutionData _principalSolution = null;

                            //Check solution
                            if (_channel.ExistingSolution)
                            {
                                worker.ReportProgress(-1, "Getting existing solution...");
                                _principalSolution = _channel.Solution;
                                LogInfo("The existing solution has been obtained: " + _principalSolution.DisplayName);
                            }
                            else
                            {
                                worker.ReportProgress(-1, "Checking solution...");
                                if (!CRMOperations.ExistsSolution(Service, _channel.SolutionName.Replace(" ", string.Empty).ToLower()))
                                {
                                    worker.ReportProgress(-1, "Creating solution...");
                                    Guid _idSolucion = CRMOperations.CreateSolution(Service, _channel.SolutionName.Replace(" ", string.Empty).ToLower(), _channel.SolutionName, _channel.Publisher.Id);
                                    _principalSolution = new SolutionData()
                                    {
                                        Id = _idSolucion,
                                        DisplayName = _channel.SolutionName,
                                        Name = _channel.SolutionName.Replace(" ", string.Empty).ToLower()
                                    };
                                    LogInfo("Se ha creado una nueva solución: " + _principalSolution.DisplayName);
                                }
                                else
                                    LogWarning("The solution " + _channel.SolutionName + " already exists. The creation process is canceled.");
                            }

                            worker.ReportProgress(-1, "Checking the configuration relationship...");
                            //Verifying the configuration entity
                            if (!CRMOperations.ExistsRelationship(Service, "msdyn_extendedentityid_" + _channel.ConfigurationEntity.LogicalName))
                            {
                                worker.ReportProgress(-1, "Creating a configuration relationship...");
                                CRMOperations.CreateRelationship(Service, _channel.ConfigurationEntity.LogicalName, _principalSolution.Name);
                                LogInfo("The relationship for the configuration entity has been created");
                            }
                            else
                                LogWarning("The relationship for the configuration entity already exists");

                            worker.ReportProgress(-1, "Getting configuration metadata...");
                            RetrieveEntityResponse _configurationMetadata = CRMOperations.GetMetadata(Service, _channel.ConfigurationEntity.LogicalName);
                            LogInfo("The configuration entity metadata information has been obtained.");

                            // Using the before method before adding the component
                            worker.ReportProgress(-1, "Checking the (Configuration) component of the solution...");
                            if (!CRMOperations.ExistsSolutionComponent(Service, _configurationMetadata.EntityMetadata.MetadataId.Value, _principalSolution.Id))
                            {
                                worker.ReportProgress(-1, "Adding configuration to the solution...");
                                CRMOperations.AddSolutionComponent(Service, _configurationMetadata.EntityMetadata.MetadataId.Value, 1, _principalSolution.Name, true);
                                LogInfo("The necessary components (Configuration) were added to the solution.");
                            }
                            else
                                LogWarning("The necessary components (Configuration) in the solution are already added.");

                            //Checking the information for the text editor
                            if (_channel.EditorEntity != null && _channel.ConfigurationForm != null)
                            {
                                worker.ReportProgress(-1, "Getting message editor metadata...");
                                RetrieveEntityResponse _configurationMetadataEditor = CRMOperations.GetMetadata(Service, _channel.EditorEntity.LogicalName);

                                worker.ReportProgress(-1, "Checking the solution component (Editor)...");
                                if (!CRMOperations.ExistsSolutionComponent(Service, _configurationMetadataEditor.EntityMetadata.MetadataId.Value, _principalSolution.Id))
                                {
                                    worker.ReportProgress(-1, "Adding the message editor to the solution...");
                                    CRMOperations.AddSolutionComponent(Service, _configurationMetadataEditor.EntityMetadata.MetadataId.Value, 1, _principalSolution.Name, true);
                                    LogInfo("Added the necessary components (Editor) to the solution.");
                                }
                                else
                                    LogWarning("The necessary components (Editor) in the solution are already added.");
                            }

                            //Checking the custom API
                            worker.ReportProgress(-1, "Checking the solution component (custom API)...");
                            if (!CRMOperations.ExistsSolutionComponent(Service, _channel.CustomAPI.Id, _principalSolution.Id))
                            {
                                worker.ReportProgress(-1, "Adding the custom API to the solution...");
                                CRMOperations.AddSolutionComponent(Service, _channel.CustomAPI.Id, 10027, _principalSolution.Name, true);
                                LogInfo("Added necessary components (Custom API) to the solution.");
                            }
                            else
                                LogWarning("The necessary components (custom API) in the solution are already added.");

                            //Checking adicional components
                            if (_channel.AdditionalEntities != null)
                            {
                                foreach (AdditionalEntity _item in _channel.AdditionalEntities)
                                {
                                    worker.ReportProgress(-1, "Getting Additional component(" + _item.DisplayName + ") metadata...");
                                    RetrieveEntityResponse _additionalMetadata = CRMOperations.GetMetadata(Service, _item.LogicalName);

                                    worker.ReportProgress(-1, "Checking the solution component (Additional component: " + _item.DisplayName + ")...");
                                    if (!CRMOperations.ExistsSolutionComponent(Service, _additionalMetadata.EntityMetadata.MetadataId.Value, _principalSolution.Id))
                                    {
                                        worker.ReportProgress(-1, "Adding the " + _item.DisplayName + " to the solution...");
                                        CRMOperations.AddSolutionComponent(Service, _additionalMetadata.EntityMetadata.MetadataId.Value, 1, _principalSolution.Name, true);
                                        LogInfo("Added necessary components (" + _item.DisplayName + ") to the solution.");
                                    }
                                    else
                                        LogWarning("The necessary components (" + _item.DisplayName + ") in the solution are already added.");
                                }
                            }

                            //Published if necessary
                            if (_channel.PublishStart)
                            {
                                worker.ReportProgress(-1, "Publishing the solution...");
                                CRMOperations.PublishSolution(Service, _principalSolution.Name);
                                LogInfo("The solution created has been published");
                            }
                            //Get the necessary solution
                            worker.ReportProgress(-1, "Exporting the solution...");
                            CRMOperations.ExportSolution(Service, _principalSolution.Name, TempPath, false, false);
                            LogInfo("The solution has been exported");

                            //Unzip the solution
                            worker.ReportProgress(-1, "Extracting the solution...");
                            FileOperations.ExtractSolution(TempPath, _principalSolution.Name + ".zip");
                            LogInfo("The solution has been extracted from the zip file");

                            //Get the XML document
                            worker.ReportProgress(-1, "Reading XML...");
                            XmlDocument _document = GeneralOperations.GetDocument(TempPath, _principalSolution.Name);
                            LogInfo("The XML document has been read successfully");

                            //Get the necessary data for the XML
                            //Message parts
                            XmlElement _xmlMessageParts = null;
                            if (_channel.MessagesParts.Count > 0)
                            {
                                worker.ReportProgress(-1, "Building XML (Message Parts)...");
                                _xmlMessageParts = GeneralOperations.CreateMessageParts(_document, _channel.MessagesParts);
                            }
                            LogInfo("The message parts have been created correctly.");

                            //Alternative language locale
                            List<Locale> _listaLocale = new List<Locale>();
                            if (_channel.LabelAlternative != null)
                            {
                                worker.ReportProgress(-1, "Getting locale in " + mySettings.LanguageALTName + "...");
                                Locale _alternative = new Locale()
                                {
                                    ChannelId = _channel.ChannelId,
                                    Id = _channel.LocaleIdALT,
                                    LanguageId = mySettings.Lcid.Value
                                };

                                string _json = "{" +
                                                 "\"ChannelDefinition.DisplayName\":  \"" + _channel.LabelAlternative.Name + "\"," +
                                                 "\"ChannelDefinition.Description\":  \"" + GeneralOperations.RemoveLineBreaksRegex(_channel.LabelAlternative.Description) + "\"," +
                                                 "\"ChannelDefinition.SpecialConsentLabel\":  \"" + GeneralOperations.RemoveLineBreaksRegex(_channel.LabelAlternative.SpecialConsent) + "\"";

                                //Checking message part tags
                                string _jsonPartsMessage = "";
                                foreach (MessagePartData _item in _channel.MessagesParts)
                                {
                                    if (!string.IsNullOrEmpty(_item.LabelNameALT) && !string.IsNullOrEmpty(_item.LabelDescriptionALT))
                                    {
                                        _jsonPartsMessage += ",";
                                        _jsonPartsMessage += "\"ChannelMessagePart." + _item.Name + ".DisplayName\":  \"" + _item.LabelNameALT + "\"," +
                                                "\"ChannelMessagePart." + _item.Name + ".Description\":  \"" + GeneralOperations.RemoveLineBreaksRegex(_item.LabelDescriptionALT) + "\"";
                                    }
                                }
                                _json += _jsonPartsMessage + "}";
                                _alternative.JSONContent = _json;
                                _listaLocale.Add(_alternative);
                            }

                            //English locale
                            if (_channel.LabelEnglish != null)
                            {
                                worker.ReportProgress(-1, "Getting English Locale...");
                                Locale _english = new Locale()
                                {
                                    ChannelId = _channel.ChannelId,
                                    Id = _channel.LocaleIdENG,
                                    LanguageId = 1033
                                };

                                string _json = "{" +
                                                 "\"ChannelDefinition.DisplayName\":  \"" + _channel.LabelEnglish.Name + "\"," +
                                                 "\"ChannelDefinition.Description\":  \"" + GeneralOperations.RemoveLineBreaksRegex(_channel.LabelEnglish.Description) + "\"," +
                                                 "\"ChannelDefinition.SpecialConsentLabel\":  \"" + GeneralOperations.RemoveLineBreaksRegex(_channel.LabelEnglish.SpecialConsent) + "\"";

                                //Checking message part tags
                                string _jsonPartsMessage = "";
                                foreach (MessagePartData _item in _channel.MessagesParts)
                                {
                                    if (!string.IsNullOrEmpty(_item.LabelNameENG) && !string.IsNullOrEmpty(_item.LabelDescriptionENG))
                                    {
                                        _jsonPartsMessage += ",";
                                        _jsonPartsMessage += "\"ChannelMessagePart." + _item.Name + ".DisplayName\":  \"" + _item.LabelNameENG + "\"," +
                                                "\"ChannelMessagePart." + _item.Name + ".Description\":  \"" + GeneralOperations.RemoveLineBreaksRegex(_item.LabelDescriptionENG) + "\"";
                                    }
                                }
                                _json += _jsonPartsMessage + "}";
                                _english.JSONContent = _json;
                                _listaLocale.Add(_english);
                            }

                            //Locales
                            XmlElement _xmlLocales = null;
                            if (_listaLocale.Count > 0)
                            {
                                worker.ReportProgress(-1, "Building XML (Localizations)...");
                                _xmlLocales = GeneralOperations.CreateLocalization(_document, _listaLocale);
                            }
                            LogInfo("The locations have been created correctly");

                            //Channel           
                            worker.ReportProgress(-1, "Building XML (Channel)...");
                            ChannelXmlData _data = new ChannelXmlData(_channel);
                            XmlElement _xmlChannel = GeneralOperations.CreateChannel(_document, _data);
                            LogInfo("The channel has been created successfully");

                            //Display XML in logs
                            List<XmlElement> _processsXML = new List<XmlElement>();
                            if (_xmlMessageParts != null)
                                LogInfo("Messages Parts: " + _xmlMessageParts.OuterXml);
                            if (_xmlLocales != null)
                                LogInfo("Locales: " + _xmlLocales.OuterXml);
                            LogInfo("Channel : " + _xmlChannel.OuterXml);

                            worker.ReportProgress(-1, "Editing XML in the solution...");
                            GeneralOperations.EditXMLCustomization(_document, TempPath, _principalSolution.Name);
                            LogInfo("Customizations.xml file edited successfully");

                            //Build Solution
                            worker.ReportProgress(-1, "Building the new solution...");
                            string _newSolution = _principalSolution.Name.Replace(" ", string.Empty).ToLower() + "_edit.zip";
                            FileOperations.ZipSolution(TempPath, _principalSolution.Name + ".zip", _newSolution);
                            LogInfo("New solution generated in a zip file");

                            if (_dataProcess.CopySolution)
                            {
                                worker.ReportProgress(-1, "Copying solution...");
                                FileOperations.CopyFile(TempPath, _newSolution, _dataProcess.PathCopy);
                                LogInfo("Solution file copied to: " + _dataProcess.PathCopy);
                            }

                            if (_dataProcess.ImporSolution)
                            {
                                //Import the solution
                                worker.ReportProgress(-1, "Importing the solution...");
                                CRMOperations.ImportSolution(Service, _principalSolution.Name.Replace(" ", string.Empty).ToLower() + "_edit.zip", TempPath);
                                LogInfo("Solution imported successfully");

                                //Publish the solution
                                worker.ReportProgress(-1, "Publishing the solution...");
                                CRMOperations.PublishSolution(Service, _principalSolution.Name.Replace(" ", string.Empty).ToLower());
                                LogInfo("Solution published successfully");
                            }

                            worker.ReportProgress(-1, "Deleting temporary files...");
                            string _firstSolution = TempPath + "\\" + _principalSolution.Name.Replace(" ", string.Empty).ToLower() + ".zip";
                            string _editedSolution = TempPath + "\\" + _principalSolution.Name.Replace(" ", string.Empty).ToLower() + "_edit.zip";
                            string _solutionDirectory = TempPath + "\\" + _principalSolution.Name.Replace(" ", string.Empty).ToLower();
                            FileOperations.DeleteFile(_firstSolution);
                            FileOperations.DeleteFile(_editedSolution);
                            FileOperations.DeleteDirectory(_solutionDirectory);
                            LogInfo("Temporary files deleted...");

                            worker.ReportProgress(-1, "Process completed!");
                            _generatedSolution = _principalSolution.Name.Replace(" ", string.Empty).ToLower();

                            _response.Message = "The solution has been generated successfully.";
                            _response.State = true;
                            eventWorker.Result = _response;
                        }
                        catch (Exception _ex)
                        {
                            LogError("It happened when generating the channel solution: " + _ex.Message + " " + _ex.StackTrace);
                            _response.Message = "It happened when generating the channel solution: " + _ex.Message;
                            _response.State = false;
                            _response.Exception = _ex;
                            eventWorker.Result = _response;
                        }
                    },
                    ProgressChanged = eventWorker =>
                    {
                        SetWorkingMessage(eventWorker.UserState.ToString());
                    },
                    PostWorkCallBack = eventWorker =>
                    {
                        ResponseOperations _response = (ResponseOperations)eventWorker.Result;
                        if (_response.State)
                        {
                            BtnExportSolution.Enabled = true;
                            MessageBox.Show(_response.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                            MessageBox.Show(_response.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    },
                    AsyncArgument = _channelParameter
                });

            }
        }

        /// <summary>
        /// Open the about window
        /// </summary>
        public void ShowAboutDialog()
        {
            About _aboutForm = new About();
            _aboutForm.ShowDialog();
        }

        /// <summary>
        /// This event occurs when the copy solution checkbox changes value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckCopySolution_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckCopySolution.Checked)
            {
                TxtCopySolution.Enabled = true;
                BtnCopySolution.Enabled = true;
                TxtCopySolution.Text = "";
            }
            else
            {
                TxtCopySolution.Enabled = false;
                BtnCopySolution.Enabled = false;
                TxtCopySolution.Text = "";
            }
        }

        /// <summary>
        ///  This event occurs when the copy solution button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCopySolution_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog _folderDialog = new FolderBrowserDialog())
            {
                _folderDialog.ShowNewFolderButton = true;
                if (_folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string _directory = _folderDialog.SelectedPath;
                    TxtCopySolution.Text = _directory;
                }
            }
        }
    }
}