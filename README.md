Twitch Plays Swarm
==================
Programming: Andreas Bresser
Graphics: Martin Schemmel

About the game
--------------
This game was developed as serious game during the Ocean Game Jam 2017 in Cologne.

The idea behind is that a "swarm" of online users has the power to control a simulated swarm of fish. 

The users can democratically decide where the swarm is heading. 
After a timer has count down the swarm goes where most user wants it to go. 
This lets the swarm either grow 
(when the water quality is good and there is nothing else disturbing the fish) 
or shrink 
(when the water quality is bad and/or there are dangers for the swarm like fishing boats).

Screenshots
-----------
![Screenshot 1](screenshots/screen1.png?raw=true "Screenshot 1")

![Screenshot 2](screenshots/screen2.png?raw=true "Screenshot 2")

![Screenshot 3](screenshots/screen3.png?raw=true "Screenshot 3")

![Screenshot 4](screenshots/screen4.png?raw=true "Screenshot 4")

Configure Local/Offline
-----------------------
You can play this game offline. This is the default so you can play.
Just click on the GameObject GameLogic and make sure the checkbox "offline mode" is set.
Than you can play the game pressing the play button or compile&run it.

Play In Offline mode
--------------------
As in the online mode the player has a specific time to react. 
Every time one of the arrow-keys is pressed it counts as a single vote by a new user.
The user can choose if the swarm should go left (left-arrow key), right (right-arrow key) or forward (up-arrow key).
The swarm reacts on the most voted direction.

Online
---------------
This game is played with twitch. 
Make sure the checkbox "Offline Mode" on the GameObject "GameLogic" is deactivated.
Then click on the GameObject "TwitchConnection" and set your channel name (WITHOUT "#") 
and the Oauth-key you can generate on the twitch site (INCLUDING "oauth:").
When you start the game it connects to the Twitch-API IRC Server and listens on commands in your channel.

Every Player in your Channel has a specific time to react.
You can set it in the "GameLogic" GameObject under "Reset After".
suggest setting this to at least 30 because of the delay in online streaming.

Make sure you stream the game using a Broadcasting software (like OBS).
If you don't want to run the game in fullscreen all the time make sure it is compiled
as background service (this is already set in this project)

Play In Online mode
--------------------
While the clock in the bottom-right corner counts down every user in your chat can vote on the direction he wants the swarm to go.
He can do so by typing "left", "right" or "forward". During the countdown he can change his mind and switch to another direction by just typing it.

The user can choose if the swarm should go left (left-arrow key), right (right-arrow key) or forward (up-arrow key).
The swarm reacts on the most voted direction.