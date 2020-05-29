// Copyright (c) Cingulara LLC 2019 and Tutela LLC 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using NATS.Client;
using System.Text;
using NLog;
using NLog.Config;
using openrmf_msg_system.Models;
using openrmf_msg_system.Data;
using MongoDB.Bson;
using openrmf_msg_system.Classes;
using Newtonsoft.Json;

namespace openrmf_msg_system
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.Configuration = new XmlLoggingConfiguration($"{AppContext.BaseDirectory}nlog.config");

            var logger = LogManager.GetLogger("openrmf_msg_system");
            
            // Create a new connection factory to create a connection.
            ConnectionFactory cf = new ConnectionFactory();
            // add the options for the server, reconnecting, and the handler events
            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.MaxReconnect = -1;
            opts.ReconnectWait = 2000;
            opts.Name = "openrmf-msg-system";
            opts.Url = Environment.GetEnvironmentVariable("NATSSERVERURL");
            opts.AsyncErrorEventHandler += (sender, events) =>
            {
                logger.Info("NATS client error. Server: {0}. Message: {1}. Subject: {2}", events.Conn.ConnectedUrl, events.Error, events.Subscription.Subject);
            };

            opts.ServerDiscoveredEventHandler += (sender, events) =>
            {
                logger.Info("A new server has joined the cluster: {0}", events.Conn.DiscoveredServers);
            };

            opts.ClosedEventHandler += (sender, events) =>
            {
                logger.Info("Connection Closed: {0}", events.Conn.ConnectedUrl);
            };

            opts.ReconnectedEventHandler += (sender, events) =>
            {
                logger.Info("Connection Reconnected: {0}", events.Conn.ConnectedUrl);
            };

            opts.DisconnectedEventHandler += (sender, events) =>
            {
                logger.Info("Connection Disconnected: {0}", events.Conn.ConnectedUrl);
            };
            
            // Creates a live connection to the NATS Server with the above options
            IConnection c = cf.CreateConnection(opts);

            // update all artifact records w/ the new title of the system
            // we save the title here for quicker data display when pulling a checklist record and metadata
            // openrmf.system.update.{systemGroupId} -- update the title in the body for the Artifacts (used for quicker reading)
            EventHandler<MsgHandlerEventArgs> updateSystemChecklistTitles = (sender, natsargs) =>
            {
                try {
                    // print the message
                    logger.Info("NATS Msg Checklists: {0}", natsargs.Message.Subject);
                    logger.Info("NATS Msg system data: {0}",Encoding.UTF8.GetString(natsargs.Message.Data));
                    
                    SystemGroup sg;
                    // setup the MondoDB connection
                    Settings s = new Settings();
                    s.ConnectionString = Environment.GetEnvironmentVariable("MONGODBCONNECTION");
                    s.Database = Environment.GetEnvironmentVariable("MONGODB");
                    // setup the database repo for systems
                    SystemGroupRepository _systemGroupRepo = new SystemGroupRepository(s);
                    // setup the database repo for the Artifacts to cycle through
                    ArtifactRepository _artifactRepo = new ArtifactRepository(s);
                    string systemGroupId = natsargs.Message.Subject.Replace("openrmf.system.update.","");
                    string systemTitle = Encoding.UTF8.GetString(natsargs.Message.Data);
                    bool artifactUpdate;
                    if (!string.IsNullOrEmpty(systemGroupId) && !string.IsNullOrEmpty(systemTitle)) {
                        sg = _systemGroupRepo.GetSystemGroup(systemGroupId).Result;
                        if (sg != null) {
                            IEnumerable<Artifact> arts;
                            arts = _artifactRepo.GetSystemArtifacts(systemGroupId).Result;
                            foreach (Artifact a in arts) {
                                // pull each record, update the systemTitle, save the artifact
                                a.systemTitle = systemTitle;
                                artifactUpdate = _artifactRepo.UpdateArtifact(a.InternalId.ToString(),a).Result;
                                if (!artifactUpdate) {
                                    logger.Warn("Warning: did not update the system title in Artifacts {0}: {1}", a.InternalId.ToString(), systemTitle);        
                                }
                            }
                        } 
                        else {
                            logger.Warn("Warning: Invalid System Group ID when updating the system title in artifacts {0}", natsargs.Message.Subject);
                        }
                    }
                    else {
                        logger.Warn("Warning: No System Group ID or Title when updating the system title in system {0} title {1}", natsargs.Message.Subject, Encoding.UTF8.GetString(natsargs.Message.Data));
                    }
                }
                catch (Exception ex) {
                    // log it here
                    logger.Error(ex, "Error retrieving system group record for system group id {0}", natsargs.Message.Subject);
                }
            };

            // update the # of checklists up or down based on the add or delete
            // openrmf.system.count.add -- increment the # of checklists in a system
            // openrmf.system.count.delete -- decrement the # of checklists in a system
            EventHandler<MsgHandlerEventArgs> updateSystemChecklistCount = (sender, natsargs) =>
            {
                try {
                    // print the message
                    logger.Info("NATS Msg Checklists: {0}", natsargs.Message.Subject);
                    logger.Info("NATS Msg system data: {0}",Encoding.UTF8.GetString(natsargs.Message.Data));
                    
                    SystemGroup sg;
                    // setup the MondoDB connection
                    Settings s = new Settings();
                    s.ConnectionString = Environment.GetEnvironmentVariable("MONGODBCONNECTION");
                    s.Database = Environment.GetEnvironmentVariable("MONGODB");
                    // setup the database repo
                    SystemGroupRepository _systemGroupRepo = new SystemGroupRepository(s);
                    sg = _systemGroupRepo.GetSystemGroup(Encoding.UTF8.GetString(natsargs.Message.Data)).Result;

                    if (sg != null) {
                        if (natsargs.Message.Subject.EndsWith(".add")) {
                            var result = _systemGroupRepo.IncreaseSystemGroupCount(sg.InternalId.ToString());
                        }
                        else if (natsargs.Message.Subject.EndsWith(".delete")) {
                            var myresult = _systemGroupRepo.DecreaseSystemGroupCount(sg.InternalId.ToString());
                        }
                    } 
                    else {
                        logger.Warn("Warning: bad System Group ID when updating the checklist count {0}", Encoding.UTF8.GetString(natsargs.Message.Data));
                    }
                }
                catch (Exception ex) {
                    // log it here
                    logger.Error(ex, "Error retrieving system group record for system group id {0}", Encoding.UTF8.GetString(natsargs.Message.Data));
                }
            };
            
            // update the date for the last system compliance ran
            EventHandler<MsgHandlerEventArgs> updateSystemComplianceDate = (sender, natsargs) =>
            {
                try {
                    // print the message
                    logger.Info("NATS Msg Checklists: {0}", natsargs.Message.Subject);
                    logger.Info("NATS Msg system data: {0}",Encoding.UTF8.GetString(natsargs.Message.Data));
                    
                    SystemGroup sg;
                    // setup the MondoDB connection
                    Settings s = new Settings();
                    s.ConnectionString = Environment.GetEnvironmentVariable("MONGODBCONNECTION");
                    s.Database = Environment.GetEnvironmentVariable("MONGODB");
                    // setup the database repo
                    SystemGroupRepository _systemGroupRepo = new SystemGroupRepository(s);
                    sg = _systemGroupRepo.GetSystemGroup(Encoding.UTF8.GetString(natsargs.Message.Data)).Result;
                    if (sg != null) {
                        sg.lastComplianceCheck = DateTime.Now;
                        // update the date and get back to work!
                        var result = _systemGroupRepo.UpdateSystemGroup(Encoding.UTF8.GetString(natsargs.Message.Data),sg);
                    } 
                    else {
                        logger.Warn("Warning: bad System Group ID when updating the system group for Compliance Generation date {0}", Encoding.UTF8.GetString(natsargs.Message.Data));
                    }
                }
                catch (Exception ex) {
                    // log it here
                    logger.Error(ex, "Error updating the system group record for the Compliance Generation date {0}", Encoding.UTF8.GetString(natsargs.Message.Data));
                }
            };
            // openrmf.system.compliance 

            EventHandler<MsgHandlerEventArgs> getSystemGroupRecord = (sender, natsargs) =>
            {
                try {
                    // print the message
                    logger.Info("NATS Msg Checklists: {0}", natsargs.Message.Subject);
                    logger.Info("NATS Msg system data: {0}",Encoding.UTF8.GetString(natsargs.Message.Data));
                    
                    SystemGroup sg;
                    // setup the MondoDB connection
                    Settings s = new Settings();
                    s.ConnectionString = Environment.GetEnvironmentVariable("MONGODBCONNECTION");
                    s.Database = Environment.GetEnvironmentVariable("MONGODB");
                    // setup the database repo
                    SystemGroupRepository _systemGroupRepo = new SystemGroupRepository(s);
                    sg = _systemGroupRepo.GetSystemGroup(Encoding.UTF8.GetString(natsargs.Message.Data)).Result;
                    if (sg != null) {
                        string msg = JsonConvert.SerializeObject(sg);
                        // publish back out on the reply line to the calling publisher
                        logger.Info("Sending back compressed System Group Record");
                        c.Publish(natsargs.Message.Reply, Encoding.UTF8.GetBytes(Compression.CompressString(msg)));
                        c.Flush(); // flush the line
                    } 
                    else {
                        logger.Warn("Warning: bad System Group ID when requesting a System Group record {0}", Encoding.UTF8.GetString(natsargs.Message.Data));
                        c.Publish(natsargs.Message.Reply, Encoding.UTF8.GetBytes(Compression.CompressString(JsonConvert.SerializeObject(new SystemGroup()))));
                        c.Flush(); // flush the line
                    }
                }
                catch (Exception ex) {
                    // log it here
                    logger.Error(ex, "Error updating the system group record for the Compliance Generation date {0}", Encoding.UTF8.GetString(natsargs.Message.Data));
                }
            };

            logger.Info("setting up the OpenRMF System Update on Title subscription");
            IAsyncSubscription asyncSystemChecklistTitle = c.SubscribeAsync("openrmf.system.update.>", updateSystemChecklistTitles);
            logger.Info("setting up the OpenRMF System # checklists subscription");
            IAsyncSubscription asyncSystemChecklistCount = c.SubscribeAsync("openrmf.system.count.>", updateSystemChecklistCount);
            logger.Info("setting up the OpenRMF System Compliance generation date");
            IAsyncSubscription asyncSystemComplianceDate = c.SubscribeAsync("openrmf.system.compliance", updateSystemComplianceDate);
            logger.Info("setting up the OpenRMF System subscription");
            IAsyncSubscription asyncSystemGroupRecord = c.SubscribeAsync("openrmf.system", getSystemGroupRecord);
        }
        private static ObjectId GetInternalId(string id)
        {
            ObjectId internalId;
            if (!ObjectId.TryParse(id, out internalId))
                internalId = ObjectId.Empty;
            return internalId;
        }
    }
}
