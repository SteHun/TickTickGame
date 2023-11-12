# Everything we added
Note that this only includes changes visible while playing. There may be more changes in the code. 
* Better sound effects
* We fixed a bug where you could still win, even after losing, during the animation
* A camera, UI elements stay in the same place on screenspace and there is a bounding box
* A pause screen which can be accessed by pressing quit 
* We improved the physics with coyote time, jump buffers and different jump strengths
* The goal is now slightly transparent when not all drops have been collected yet
* A visual indication when stepping on hot tile
* Better explosion animation
* A level editor which can make, save, load and play levels
* Parallax scrolling
* Cloud count is based on level width.
* You can jump on rockets to destroy them, holding space gives a bigger bounce
* 2nd line in level data is now reserved for time the player has to complete the level
* Made a different collision detection, where you can decide how forgiving it should be (implemented on enemies)
* Buttons can now have custom text and dynamically resize.
* Textboxes can now dynamically resize
* Added two (bad) extra difficult levels to demonstrate scrolling and parallax
* Flame enemies get different colors depending on their behaviour
* The jump animation looks a bit different for some reason (this was not on purpose but it's fine)
* A quit button in the main menu
* A menu to play, load or delete custom levels
* A speed tile that temporarily gives the player a speed boost