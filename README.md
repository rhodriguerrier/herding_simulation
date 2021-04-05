# Herding Behaviour Visualisation

An implementation of boid algorithms to visualise herding behaviour of buffalo.

There are two main scripts that make up the project.
The first, GenerateHerd.cs, is a simple for loop to instantiate copies of the buffalo model
at different positions and rotaions. This is assigned to the plane object.
The second script, HerdAgent.cs, controls the core functionalities of the boids
(separation, alignment, cohesion and ray casting for obstacle avoidance).

Currently, the individual buffalo do not physically interact with on another.
This means that they occasionally overlap when packed tightly. Furthermore, there
is currently not animations for the models.

Potential updates:
- Add animation to buffalo legs.
- Have the agents flee when a character approaches too quickly.

![Herd Behaviour Simulation](https://github.com/rhodriguerrier/herding_simulation/blob/main/herding_example.PNG?raw=true)