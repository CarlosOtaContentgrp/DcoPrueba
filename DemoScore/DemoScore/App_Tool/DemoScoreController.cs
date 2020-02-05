using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace DemoScore.App_Tool
{
    public class DemoScoreController : Controller
    {

        private List<Msg> MsgList = new List<Msg>();
        // GET: DemoScore
        protected void Message(string message, MessageType type = MessageType.Success, string title = "")
        {
            if (string.IsNullOrEmpty(title))
            {
                switch (type)
                {
                    case MessageType.Success:
                        title = "Exitoso !!!";
                        break;
                    case MessageType.Info:
                        title = "Información";
                        break;
                    case MessageType.Warning:
                        title = "Alerta !!!";
                        break;
                    case MessageType.Danger:
                        title = "Alerta !!!";
                        break;
                    case MessageType.Smile:
                        title = "Exitoso !!!";
                        break;
                    case MessageType.Sad:
                        title = "Error !!!";
                        break;
                    default:
                        title = "Mensaje";
                        break;
                }
            }

            MsgList.Add(
                new Msg
                {
                    IsNotification = false,
                    Text = message,
                    Title = title,
                    Type = type
                });

            ViewBag.MessageList = MsgList;
        }

        public class Msg
        {
            /// <summary>
            /// Gets or sets a value indicating whether this instance is notification.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is notification; otherwise, <c>false</c>.
            /// </value>
            public bool IsNotification { get; set; }
            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>
            /// The type.
            /// </value>
            public MessageType Type { get; set; }
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            /// <value>
            /// The title.
            /// </value>
            public string Title { get; set; }
            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            /// <value>
            /// The text.
            /// </value>
            public string Text { get; set; }
        }
        /// <summary>
        /// The enum that enunciate the messages types
        /// </summary>
        public enum MessageType
        {
            Success,
            Info,
            Warning,
            Danger,
            Smile,
            Sad
        }
    }
}