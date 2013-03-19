using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class PlayerModel
    {
        bool _isShooting;
        bool _isGunJammed;

        public bool isAlive;
        public int playerNumber; //to identify player turn and act as a key against component value

        public PlayerModel()
        {
            playerNumber = -1;
            isAlive = true;
            _isShooting = false;
            _isGunJammed = false;
        }

        public void  Initialise()
        {
             isAlive = true;
            _isShooting = false;
            _isGunJammed = false;
        }

        //read only
        public int getBinding()
        {
            return playerNumber;
        }
        public bool isShooting
        {
            get { return _isShooting; }
        }

        public bool isGunJammed
        {
            get {
                return _isGunJammed;
            }
            set { 
                _isGunJammed = value;
                if (_isGunJammed) _isShooting = false;
            }
        }

        public void processPlayerInput(int forPlayerNumber)
        {
            if (playerNumber != forPlayerNumber) return;
            _isShooting = true;
        }
    }
}
