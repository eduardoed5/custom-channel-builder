using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using CreatorChannelsXrmToolbox.Model;
using Microsoft.Crm.Sdk.Messages;

namespace CreatorChannelsXrmToolbox
{
    /// <summary>
    /// This class allows the handling of operations in Dynamics 365
    /// </summary>
    public class CRMOperations
    {
        /// <summary>
        /// Gets the list of active publishers
        /// </summary>
        /// <param name="service">API Service for Dynamics 365</param>
        /// <returns>List of publishers</returns>
        public static List<Publisher> GetPublishers(IOrganizationService service)
        {
            List<Publisher> _list = new List<Publisher>();
            QueryExpression _query = new QueryExpression("publisher")
            {
                ColumnSet = new ColumnSet("publisherid", "uniquename", "friendlyname", "customizationprefix", "customizationoptionvalueprefix")
            };
            EntityCollection _response = service.RetrieveMultiple(_query);
            foreach (Entity _item in _response.Entities)
            {
                Publisher _publisher = new Publisher
                {
                    Id = new Guid(_item["publisherid"].ToString()),
                    Name = _item["friendlyname"].ToString(),
                    UniqueName = _item["uniquename"].ToString(),
                    Prefix = _item["customizationprefix"].ToString(),
                    Numeration = (int)_item["customizationoptionvalueprefix"]
                };
                _list.Add(_publisher);
            }
            _list.Sort((x, y) => x.Name.CompareTo(y.Name));
            return _list;
        }

        /// <summary>
        /// Gets a list of the entities in the organization
        /// </summary>
        /// <param name="service">API Service for Dynamics 365</param>
        /// <param name="codeLanguage">Language to use for display names</param>
        /// <param name="alternativeLanguageCode">Alternative language to use for display names</param>
        /// <returns>List of entities</returns>
        public static List<EntityData> GetEntities(IOrganizationService service, int codeLanguage, int alternativeLanguageCode)
        {
            List<EntityData> _list = new List<EntityData>();
            RetrieveAllEntitiesRequest _request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Entity,
                RetrieveAsIfPublished = true
            };

            RetrieveAllEntitiesResponse _respuesta = (RetrieveAllEntitiesResponse)service.Execute(_request);
            foreach (EntityMetadata _item in _respuesta.EntityMetadata)
            {
                EntityData _infoEntity = new EntityData
                {
                    LogicalName = _item.LogicalName
                };

                foreach (LocalizedLabel label in _item.DisplayName.LocalizedLabels)
                {
                    if (label.LanguageCode == codeLanguage)
                        _infoEntity.DisplayName = label.Label;
                    else
                    {
                        if (string.IsNullOrEmpty(_infoEntity.DisplayName) && label.LanguageCode == alternativeLanguageCode)
                            _infoEntity.DisplayName = label.Label;
                    }
                }
                _infoEntity.Code = _item.ObjectTypeCode ?? 0;
                _infoEntity.IsCustomizable = _item.IsCustomEntity ?? false;
                _infoEntity.IsActivity = _item.IsActivity ?? false;

                if (string.IsNullOrEmpty(_infoEntity.DisplayName))
                    _infoEntity.DisplayName = _item.LogicalName;
                _list.Add(_infoEntity);
            }
            _list.Sort((x, y) => x.DisplayName.CompareTo(y.DisplayName));
            return _list;
        }

        /// <summary>
        /// Gets a list of the organization's custom APIs
        /// </summary>
        /// <param name="service">API Service for Dynamics 365</param>
        /// <returns>List of custom APIS</returns>
        public static List<CustomAPI> GetCustomAPIs(IOrganizationService service)
        {
            List<CustomAPI> _list = new List<CustomAPI>();
            QueryExpression _query = new QueryExpression("customapi");
            _query.ColumnSet.AddColumns("customapiid", "uniquename", "displayname", "name");
            EntityCollection _response = service.RetrieveMultiple(_query);
            foreach (Entity _item in _response.Entities)
            {
                CustomAPI _infoAPI = new CustomAPI()
                {
                    Id = new Guid(_item["customapiid"].ToString()),
                    DisplayName = _item["displayname"].ToString(),
                    Name = _item["name"].ToString(),
                    UniqueName = _item["uniquename"].ToString(),
                };
                _list.Add(_infoAPI);
            }
            _list.Sort((x, y) => x.Name.CompareTo(y.Name));
            return _list;
        }

        /// <summary>
        /// Gets a list of main forms of a specified entity
        /// </summary>
        /// <param name="service">API Service for Dynamics 365</param>
        /// <param name="entityCode">Entity code to filter</param>
        /// <returns>List of forms</returns>
        public static List<FormData> GetForms(IOrganizationService service, int entityCode)
        {
            List<FormData> _list = new List<FormData>();
            QueryExpression _query = new QueryExpression("systemform")
            {
                ColumnSet = new ColumnSet("formid", "name")
            };
            _query.Criteria.AddCondition("objecttypecode", ConditionOperator.Equal, entityCode);
            _query.Criteria.AddCondition("type", ConditionOperator.Equal, 2);
            EntityCollection _response = service.RetrieveMultiple(_query);
            foreach (Entity _item in _response.Entities)
            {
                FormData _form = new FormData()
                {
                    Id = _item.Id,
                    Name = _item["name"].ToString()
                };
                _list.Add(_form);
            }
            _list.Sort((x, y) => x.Name.CompareTo(y.Name));
            return _list;
        }

        /// <summary>
        /// Gets a list of views of a specified entity
        /// </summary>
        /// <param name="service">API Service for Dynamics 365</param>
        /// <param name="entityCode">Entity code to filter</param>
        /// <returns>List of views</returns>
        public static List<ViewData> GetViews(IOrganizationService service, int entityCode)
        {
            List<ViewData> _list = new List<ViewData>();
            QueryExpression _query = new QueryExpression("savedquery");
            _query.ColumnSet.AddColumns("name", "savedqueryid");
            _query.Criteria.AddCondition("returnedtypecode", ConditionOperator.Equal, entityCode);

            EntityCollection _response = service.RetrieveMultiple(_query);
            foreach (Entity _item in _response.Entities)
            {
                ViewData _view = new ViewData()
                {
                    Id = _item.Id,
                    Name = _item["name"].ToString()
                };
                _list.Add(_view);
            }
            _list.Sort((x, y) => x.Name.CompareTo(y.Name));
            return _list;
        }

        /// <summary>
        /// Allows you to obtain a list of solutions from a specific publisher
        /// </summary>
        /// <param name="service">API Service for Dynamics 365</param>
        /// <param name="publisherId">Publisher ID to filter</param>
        /// <returns>List of solutions</returns>
        public static List<SolutionData> GetSolutions(IOrganizationService service, Guid publisherId)
        {
            List<SolutionData> _list = new List<SolutionData>();
            QueryExpression _query = new QueryExpression("solution");
            _query.ColumnSet.AddColumns("solutionid", "uniquename", "friendlyname");
            _query.Criteria.AddCondition(new ConditionExpression("publisherid", ConditionOperator.Equal, publisherId.ToString()));
            EntityCollection _response = service.RetrieveMultiple(_query);
            foreach (Entity _item in _response.Entities)
            {
                SolutionData _solution = new SolutionData()
                {
                    Id = new Guid(_item["solutionid"].ToString()),
                    DisplayName = _item["friendlyname"].ToString(),
                    Name = _item["uniquename"].ToString(),
                };
                _list.Add(_solution);
            }
            _list.Sort((x, y) => x.Name.CompareTo(y.Name));
            return _list;
        }

        /// <summary>
        /// Indicates whether the specified solution exists
        /// </summary>
        /// <param name="service">API Service for Dynamics 365</param>
        /// <param name="solutionName">Solution name</param>
        /// <returns>True if it exists, False if it does not.</returns>
        public static bool ExistsSolution(IOrganizationService service, string solutionName)
        {
            QueryExpression _query = new QueryExpression("solution")
            {
                ColumnSet = new ColumnSet("uniquename"),
                Criteria = new FilterExpression()
            };
            _query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, solutionName);
            return service.RetrieveMultiple(_query).Entities.Count > 0;
        }

        /// <summary>
        /// Create a new solution
        /// </summary>
        /// <param name="service">API Service for Dynamics 365</param>
        /// <param name="uniqueName"> Unique name for the solution</param>
        /// <param name="name">Display name of the solution</param>
        /// <param name="publisherId">Publisher ID</param>
        /// <returns>Solution ID</returns>
        public static Guid CreateSolution(IOrganizationService service, string uniqueName, string name, Guid publisherId)
        {
            Entity _solution = new Entity("solution");
            _solution.Attributes.Add("uniquename", uniqueName.Replace(" ", string.Empty));
            _solution.Attributes.Add("friendlyname", name);
            _solution.Attributes.Add("publisherid", new EntityReference("publisher", publisherId));
            _solution.Attributes.Add("version", "1.0.0.0");
            Guid _idSolution = service.Create(_solution);
            return _idSolution;
        }

        /// <summary>
        /// Indicates whether the specified relationship exists
        /// </summary>
        /// <param name="service">API Service for Dynamics 365</param>
        /// <param name="schemaName">Schema name</param>
        /// <returns>True if it exists, False if it does not.</returns>
        public static bool ExistsRelationship(IOrganizationService service, string schemaName)
        {
            try
            {
                RetrieveRelationshipRequest _request = new RetrieveRelationshipRequest
                {
                    Name = schemaName,
                    RetrieveAsIfPublished = true
                };

                RetrieveRelationshipResponse _response = (RetrieveRelationshipResponse)service.Execute(_request);
                return _response.RelationshipMetadata != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Create the relationship with the extended configuration entity
        /// </summary>
        /// <param name="servicio">API Service for Dynamics 365</param>
        /// <param name="entity">Logical name of the extended configuration entity</param>
        /// <param name="solution">Unique name of the solution</param>
        /// <param name="isAccountEntity">Indicates whether the relationship will be created with the account configuration entity</param>
        public static void CreateRelationship(IOrganizationService servicio, string entity, string solution, bool isAccountEntity)
        {
            CreateOneToManyRequest _request = new CreateOneToManyRequest();
            OneToManyRelationshipMetadata _oneMuch = new OneToManyRelationshipMetadata
            {
                ReferencingEntity = isAccountEntity ? "msdyn_channelinstanceaccount" : "msdyn_channelinstance",
                ReferencedEntity = entity,
                ReferencingEntityNavigationPropertyName = $"msdyn_extendedentityid_{entity}",
                SchemaName = $"msdyn_extendedentityid_{entity}"
            };

            _request.OneToManyRelationship = _oneMuch;
            _request.Parameters["Lookup"] = new LookupAttributeMetadata()
            {
                SchemaName = "msdyn_extendedentityId",
                DisplayName = new Label("Extended Entity Id", 1033)
            };
            _request.SolutionUniqueName = solution.Replace(" ", string.Empty);
            servicio.Execute(_request);
        }

        /// <summary>
        /// Indicates whether the specified solution component exists
        /// </summary>
        /// <param name="service">API Service for Dynamics 365</param>
        /// <param name="idComponent">Component ID</param>
        /// <param name="idSolution">Solution ID</param>
        /// <returns>True if it exists, False if it does not.</returns>
        public static bool ExistsSolutionComponent(IOrganizationService service, Guid idComponent, Guid idSolution)
        {
            string _fetchXml = $@"
                                <fetch top='1'>
                                  <entity name='solutioncomponent'>
                                    <attribute name='objectid' />
                                    <filter>
                                      <condition attribute='objectid' operator='eq' value='{idComponent}' />
                                      <condition attribute='solutionid' operator='eq' value='{idSolution}' />
                                    </filter>
                                  </entity>
                                </fetch>";

            EntityCollection _response = service.RetrieveMultiple(new FetchExpression(_fetchXml));
            return _response.Entities.Count > 0;
        }

        /// <summary>
        /// Add a component to the solution
        /// </summary>
        /// <param name="service">API Service for Dynamics 365</param>
        /// <param name="idComponent">Component ID</param>
        /// <param name="componentType">Component type code</param>
        /// <param name="solutionName">Solution name</param>
        /// <param name="all">Indicates whether all required components are added</param>
        public static void AddSolutionComponent(IOrganizationService service, Guid idComponent, int componentType, string solutionName, bool all)
        {
            AddSolutionComponentRequest _request = new AddSolutionComponentRequest()
            {
                ComponentType = componentType,
                ComponentId = idComponent,
                SolutionUniqueName = solutionName,
                AddRequiredComponents = all
            };
            service.Execute(_request);
        }

        /// <summary>
        /// Allows you to obtain the solution component ID for a specified entity.
        /// </summary>
        /// <param name="service">API Service for Dynamics 365</param>
        /// <param name="primaryentityname">Logical name of the entity</param>
        /// <returns>Component type ID of the solution</returns>
        /// <exception cref="Exception">Exception when the records of the specified entity cannot be added directly to the solution</exception>
        public static int GetObjectTypeComponentDefinition(IOrganizationService service, string primaryentityname)
        {
            QueryExpression _query = new QueryExpression("solutioncomponentdefinition")
            {
                NoLock = true,
                ColumnSet = new ColumnSet("objecttypecode"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                       new ConditionExpression("canbeaddedtosolutioncomponents", ConditionOperator.Equal, true),
                       new ConditionExpression("primaryentityname", ConditionOperator.Equal, primaryentityname)
                    }
                }
            };

            EntityCollection _response = service.RetrieveMultiple(_query);
            if (_response.Entities.Count > 0)
                return Convert.ToInt32(_response.Entities[0]["objecttypecode"].ToString());
            else
                throw new Exception("Records of the entity with logical name: " + primaryentityname + " cannot be added directly in a solution.");
        }

        /// <summary>
        /// Publish the specified solution
        /// </summary>
        /// <param name="service">API Service for Dynamics 365</param>
        /// <param name="solutionName">Unique name of the solution</param>
        public static void PublishSolution(IOrganizationService service, string solutionName)
        {
            PublishXmlRequest _request = new PublishXmlRequest
            {
                ParameterXml = $"<importexportxml><solutions><solution>{solutionName}</solution></solutions></importexportxml>"
            };
            // Ejecutar la solicitud
            service.Execute(_request);
        }

        /// <summary>
        /// Export the indicated solution to a zip file
        /// </summary>
        /// <param name="service">API Service for Dynamics 365</param>
        /// <param name="solutionName">Unique name of the solution</param>
        /// <param name="filePath">Path where the resulting zip file will be written</param>
        /// <param name="completePath">Indicates that the path is complete, including the file name. If false, the file name will be set to the solution name.</param>
        /// <param name="managed">Indicates whether the solution will be exported as managed or not</param>
        public static void ExportSolution(IOrganizationService service, string solutionName, string filePath, bool completePath, bool managed)
        {
            ExportSolutionRequest _request = new ExportSolutionRequest
            {
                SolutionName = solutionName,
                Managed = managed
            };
            ExportSolutionResponse _respuesta = (ExportSolutionResponse)service.Execute(_request);
            if (completePath)
                FileOperations.WriteFile(filePath, _respuesta.ExportSolutionFile);
            else
                FileOperations.WriteFile(filePath, _respuesta.ExportSolutionFile, solutionName + ".zip");
        }

        /// <summary>
        /// Import the indicated solution from a zip file
        /// </summary>
        /// <param name="servicio">API Service for Dynamics 365</param>
        /// <param name="nombreSolucion">Unique name of the solution</param>
        /// <param name="rutaArchivo">Path where the solution zip file is located</param>
        public static void ImportSolution(IOrganizationService servicio, string nombreSolucion, string rutaArchivo)
        {
            byte[] _binary = FileOperations.ReadFile(rutaArchivo, nombreSolucion);
            ImportSolutionRequest _request = new ImportSolutionRequest
            {
                CustomizationFile = _binary,
                OverwriteUnmanagedCustomizations = true,
                PublishWorkflows = true
            };
            servicio.Execute(_request);
        }

        /// <summary>
        /// Obtain the metadata information of the specified entity
        /// </summary>
        /// <param name="service">API Service for Dynamics 365</param>
        /// <param name="logicalName">Logical name of the entity</param>
        /// <returns>Entity metadata</returns>
        public static RetrieveEntityResponse GetMetadata(IOrganizationService service, string logicalName)
        {
            RetrieveEntityRequest _request = new RetrieveEntityRequest
            {
                LogicalName = logicalName,
                EntityFilters = EntityFilters.Entity
            };
            return service.Execute(_request) as RetrieveEntityResponse;
        }

        /// <summary>
        /// Get the list of languages ​​supported by Dynamics 365
        /// </summary>
        /// <param name="service">API Service for Dynamics 365</param>
        /// <returns>list of languages</returns>
        public static List<LanguageData> GetLanguages(IOrganizationService service)
        {
            List<LanguageData> _list = new List<LanguageData>();
            QueryExpression _query = new QueryExpression("languagelocale")
            {
                ColumnSet = new ColumnSet("code", "localeid", "language", "name", "region")
            };

            EntityCollection _response = service.RetrieveMultiple(_query);
            foreach (Entity _item in _response.Entities)
            {
                LanguageData _infoLanguage = new LanguageData()
                {
                    ISOCode = _item["code"].ToString(),
                    Lcid = (int)_item["localeid"],
                    Name = _item["language"].ToString(),
                    DisplayName = _item["name"].ToString(),
                    Region = _item["region"].ToString()
                };
                _list.Add(_infoLanguage);
            }
            _list.Sort((x, y) => x.DisplayName.CompareTo(y.DisplayName));
            return _list;
        }

    }
}
