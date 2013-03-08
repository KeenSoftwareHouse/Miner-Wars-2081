using System.Net;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.Networking
{
    //  IMPORTANT: Never change numeric values!
    public enum MyMwcCheaterAlertType : byte
    {
        //  This is when we receive message type with unknown type or number
        UNKNOWN_MESSAGE_TYPE = 0,                       
        
        //  We have this message type in our enum, but for some reasons it isn't handled in the receive messages switch
        UNHANDLED_MESSAGE_TYPE = 1,                     

        //  When message received on client-server that has not callback specified for that type of message (e.g. LOGIN sent from player to player)
        NO_CALLBACK_SPECIFIED = 2,                      
        
        //  Message received from unknown end-point and it wasn't LOGIN or LOGIN_SS message
        MESSAGE_FROM_UNKNOWN_ENDPOINT = 3,              

        //  MAC code in the packet doesn't equal MAC code we expect from that end-point! Probably sent from different IP address or someone is trying to pretend to be other player.
        WRONG_MAC = 4,                                  
        
        //  Too big message received, we ignore it.
        MESSAGE_RECEIVED_TOO_BIG = 5,                    

        //  Exception occured during we are reading message from memory stream. Message and also exception will be ignored, but we must know about it.
        EXCEPTION_DURING_READING_MESSAGE = 6,           
        
        //  If we read byte from the message, but can't convert/case it to specified enum
        UNKNOWN_ENUM_VALUE = 7,                         
        
        //  Player has received join sector notification from server on endpoint which he already has
        DUPLICATE_ENTER_SECTOR_NOTIFICATION = 8,        

        //  When player receives message from other player, but this message is supposed to be sent only from a server. In this case callback is 
        //  specified because player should handle those message, but only if received from server (he server shutdown notification, join sector notification, etc)
        MESSAGE_RECEIVED_NOT_FROM_SERVER = 9,           
        
        //  Wrong value received (too large, not handled enum value, etc...)
        WRONG_VALUE = 10,                               
        
        //  Message received from a player who is not logged in, but this message needs logged in player otherwise it can't work 
        //  properly (user not found by endpoint). Standard handling of this scenario is that server won't send any response to the player.
        MESSAGE_FROM_PLAYER_WHO_IS_NOT_LOGGED_IN = 11,

        //  Received "start enter sector" on user who is not right now in any game
        START_ENTER_SECTOR_FOR_USER_WHO_IS_NOT_IN_GAME = 12,

        //  Received "end enter sector" on user who is not right now in any game
        END_ENTER_SECTOR_FOR_USER_WHO_IS_NOT_IN_GAME = 13,

        //  Received "leave sector" on user who is not right now in any game
        //LEAVE_SECTOR_FOR_USER_WHO_IS_NOT_IN_GAME = 14,
        
        //  If player sent message where we want to transfer between two sector that aren't adjacent - it's something we don't support and don't allow
        TRANSFERING_BETWEEN_NOT_ADJACENT_SECTORS = 15,

        //  For sequenced messages
        SEQUENCED_MESSAGE_IS_TOO_LARGE = 16,
        WHILE_PROCESSING_SEQUENCED_MESSAGE_ON_A_CHANNEL_MESSAGE_OF_DIFFERENT_TYPE_RECEIVED = 17
    }

    public static class MyMwcCheaterAlert
    {
        public static void AddAlert(MyMwcCheaterAlertType type, EndPoint cheaterAddress, string description)
        {
            MyMwcLog.WriteLine("Networking.MyMwcCheaterAlert - START");
            MyMwcLog.IncreaseIndent();

            //  Write to local log too, because if sending cheater alert won't be successful, at least user can send us the log file.
            MyMwcLog.WriteLine("Type: " + (int)type);
            MyMwcLog.WriteLine("CheaterAddress: " + ((cheaterAddress == null) ? "null" : cheaterAddress.ToString()));
            MyMwcLog.WriteLine("Description: " + description.ToString());

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("Networking.MyMwcCheaterAlert - END");
        }
    }
}
