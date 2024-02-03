The third agent builds upon the work you completed for the first and second
agent. You should use the same agent and environment theme. However, 
note in this scenario the agent has different abilities / behaviour.
You are required to develop an autonomous automated agent that represents 
a delivery or collection agent (e.g. robot) that has multiple delivery or 
collection locations. The agent should start at a home location and deliver or
collect items (e.g. parcels) from multiple goal locations. The agent should then 
return home. The agent should be able to act and re-act to obstacles and 
other instances of itself in the environment.
The agent should be able to deliver or collect an item (e.g. parcel or food) 
from up to 8 goal locations. This means you should be able to set up to 8
goal locations (e.g. delivery or collection locations) in your waypoint graph.
The agent should plan the shortest route that starts at the start location (e.g. 
delivery depot), visits each delivery or drop-off location and returns to the start 
location.

////
When the agent reaches the goal point, the ACO script is disabled and the pathfing script is enabled. The agent then returns to the starting point using the A* algorithm.
