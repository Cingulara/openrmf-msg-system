// Copyright (c) Cingulara LLC 2019 and Tutela LLC 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.

namespace openrmf_msg_system.Models
{
    // this matches the SI_DATA XML sections in the Checklist CKL file
    public class SI_DATA {

        public SI_DATA (){

        }

        public string SID_NAME { get; set;}
        public string SID_DATA { get; set; }
    }
}