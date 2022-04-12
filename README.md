# Sars-CoV-2-Simulation
As part of my bachelor thesis at the University of Osnabrueck, i developed this little program in unity to simulate the spread of the Coronavirus desease on events.

## Simulating crowds
In order to simulate crowds i developed a novel layer-based approach based on [this framework introduced by van Toll et al.](https://dspace.library.uu.nl/bitstream/handle/1874/310188/Towards_believable_crowds_A_generic_multi_level_framework_for_agent_navigation_ASCI_Open_2015.pdf?sequence=1&isAllowed=y) The simulation loop was realized by using the PPO reinforcement learning algorithm implemented in the MLAgents Toolkit. For Global Route Planning i used the a*-algorithm.

## Simulating the coronavirus
The probability of infection was calculated based mainly on [this paper published by Agrawal et al.](https://pubmed.ncbi.nlm.nih.gov/33746492/) The probability in this simulation was influenced by the distance between to agents and the kind of masks (or lack there of) they are wearing.

## Modular Approach
This program is designed in a way that interested users can also start putting together their own simulation environments, though there are some limitations. For example there is still no option to change the width of the hallways etc. because of a lack of time.
