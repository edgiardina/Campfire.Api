﻿using System;
using System.Runtime.Serialization;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Campfire.Api.Utils;

namespace Campfire.Api.Models
{
    [DataContract]
    public class Message
    {
        //      <message>
        //  <created-at type="datetime">2010-04-15T11:03:08Z</created-at>
        //  <id type="integer">23</id>
        //  <room-id type="integer">1</room-id>
        //  <user-id type="integer">1</user-id>
        //  <body>Hello room!</body>
        //  <type>TextMessage</type>
        //  <starred>false</starred>
        //</message>
        [DataMember]
        public int Id { get; set; }
        [DataMember(Name = "room_id")]
        public int RoomId { get; set; }

        [DataMember(Name = "user_id")]
        public int? UserId { get; set; }
        public string UserName { get; set; }

        public string AvatarUrl { get; set; }

        [DataMember(Name = "created_at")]
        public DateTime CreatedAt { get; set; }

        [DataMember]
        public string Body { get; set; }
        [DataMember]
        public MessageType Type { get; set; }

        [DataMember]
        public bool Starred { get; set; }

        public Upload Upload { get; set; }

        public string ImageUrl
        {
            get
            {
               if (this.Type == MessageType.UploadMessage)
               {
                   return this.Upload.FullUrl;
               }
               else if (this.Type == MessageType.TextMessage)
               {
                   //If the body of a text message is only an image URL, return the body message
                   return this.Body;
               }
               else
               {
                   return null;
               } 
            }
        }

        public bool IsStarrable
        {
            get 
            { 
                return this.Type == MessageType.TextMessage || this.Type == MessageType.PasteMessage || this.Type == MessageType.UploadMessage; 
            }
        }
    }
}