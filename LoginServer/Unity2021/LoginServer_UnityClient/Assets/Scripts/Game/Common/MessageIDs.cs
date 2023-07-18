using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rater193.scb.common
{
    public class MessageIDs
    {
        //Client message IDs
        public class client {
            public const int
                RequestLogin = 1,
                JoinGame = 2,
                MapLoadStart = 10,
                MapLoadReceiveEntities = 11,
                EntityCreate=20,
                EntityUpdatePosition = 21,
                EntityUpdateRotation = 22,
                EntityRemove =23
                ;
        }

        public class common
		{
            //public const int
            //    ;

        }

        //Server message IDs
        public class server
		{
            public const int
                InitVerifyAccount = 1,
                ClientRequestEntities = 2,
                ClientMovePlayer = 10
                ;
		}
    }
}
