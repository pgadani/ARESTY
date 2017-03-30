# ARESTY
Development Repository for the ARESTY Research Program

### Things we need to add/modify

 - More behaviors
	 - Turn these into a class of some sort to be able to store pre- and post-conditions?

 - Basic interaction like selecting character and choosing location to walk/run to works
 	 - Add multiselect
 	 - Connect with GUI to choose behavior

 - GUI for selecting characters/objects and assigning behaviors, and playing these behaviors

### Current Behaviors:
	
	- Tag
	- Meet and Return (meeting, walking around while talking, going back)
	- Guards opening gates

### Behaviors to Add:
	
	- Greet?
	- Marketplace peddling/selling
	- Guards capturing someone
	- Arguing
		- Angry gesture, Annoyed head shake, Dismissing gesture, ...

### Eventually
Add something to keep choices consistent

 - Classes for NPCs and smart objects
	 - To get their current state and be able to act on them

 - Interface for these to all interact
	 - Basically the blackboard
	 - Have an overarching game object with these scripts to manage everything