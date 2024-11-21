using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Packet type guide for the roulette game
 * 
 * Receiving
 * 0 - Request for result, "Player's bet, Player's balance, Player's decision, Wheel outcome, Player's number(if applicable)"
 * 1 - Notify server to player is exiting "Some message saying player is leaving the table"
 * 
 * Sending
 * 0 - Send the result, "Player's winnings, Player's new balance, Game outcome(win or lose), Wheel outcome(e.i. 24 Black)"
 * 
 */


namespace CasinoLibrary
{
    internal class RouletteGame
    {
        public TCPServer MMMServer;
        public PlayerInfo PlayerInfo;
        Wheel wheel;
        string? gameResult;
        
        public RouletteGame(TCPServer s, PlayerInfo p)
        {
            MMMServer = s;
            PlayerInfo = p;
            wheel = new Wheel(0);
        }

        public void listen()
        {
            //Listen for client's initial requests
            while (MMMServer.packet.PacketType != 1)
            {
                MMMServer.packet = MMMServer.receivePacket();
                runRP();
            }
        }

        private void runRP()
        {
            if (MMMServer.packet.PacketType == 0)
            {
                //Break down the request
                string[] data = MMMServer.dataPayloadString.Split(',');

                //Set the wheel to its result
                wheel.resultNum = Int32.Parse(data[3]);
                wheel.spin();

                //Update player info
                PlayerInfo.bet = Int32.Parse(data[0]);
                PlayerInfo.balance = Int32.Parse(data[1]);

                //Compare player decision with wheel outcome
                determineResult(data);

                //Send the player's winnings, new balance, and the game's result
                MMMServer.packet = MMMServer.sendPacket(0, PlayerInfo.winnings + "," + PlayerInfo.balance + "," + gameResult + "," + wheel.outcome.number + " " + wheel.outcome.colour);
            }
        }

        private void determineResult(string[] data)
        {
            switch (data[2])
            {
                case "1-18":
                    if (wheel.outcome.number > 0 && wheel.outcome.number < 19)
                    {
                        PlayerInfo.calculatePayout(1, 0);
                        gameResult = "You Win!";
                        break;
                    }
                    else
                    {
                        goto default;
                    }
                case "19-36":
                    if (wheel.outcome.number > 18 && wheel.outcome.number < 37)
                    {
                        PlayerInfo.calculatePayout(1, 0);
                        gameResult = "You Win!";
                        break;
                    }
                    else
                    {
                        goto default;
                    }
                case "Red":
                    if (wheel.outcome.colour == Colour.red)
                    {
                        PlayerInfo.calculatePayout(1, 0);
                        gameResult = "You Win!";
                        break;
                    }
                    else
                    {
                        goto default;
                    }
                case "Black":
                    if (wheel.outcome.colour == Colour.black)
                    {
                        PlayerInfo.calculatePayout(1, 0);
                        gameResult = "You Win!";
                        break;
                    }
                    else
                    {
                        goto default;
                    }
                case "Even":
                    if (wheel.outcome.type == Type.even)
                    {
                        PlayerInfo.calculatePayout(1, 0);
                        gameResult = "You Win!";
                        break;
                    }
                    else
                    {
                        goto default;
                    }
                case "Odd":
                    if (wheel.outcome.type == Type.odd)
                    {
                        PlayerInfo.calculatePayout(1, 0);
                        gameResult = "You Win!";
                        break;
                    }
                    else
                    {
                        goto default;
                    }
                case "Number":
                    if (wheel.outcome.number == Int32.Parse(data[4]))
                    {
                        PlayerInfo.calculatePayout(1, 1);
                        gameResult = "MAX WIN!";
                        break;
                    }
                    else
                    {
                        goto default;
                    }
                default:
                    PlayerInfo.calculatePayout(1, 2);
                    gameResult = "You Lose!";
                    break;
            }
        }
    }
}
