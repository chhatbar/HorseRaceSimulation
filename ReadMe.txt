Problem. Horse Racing Simulation

	There are 25 Horses that need to race on the race track. Overall objective of this game is to find out which are TOP 3 horses out of the 25	horses, and how many races need to be conducted to find the top-3 horses.
	
	The constraint is that the race track can only have FIVE horses racing at a time. Further, in each race the result is only available in the form of positions of the horses NOT timing of each horse. That is, the result ONLY contains the knowledge about which horse came first, second, third, etc. not how much time each took.
	
	Target is to develop a system that simulates this racing game and determines top 3 horses out of the bulk of 25 horses by conducting the races of	five-horses at a time. Find out how many total number of races required to find the top 3 horses in this particular scenario.
	
	The number of required races in this particular scenario are NOT MORE than 7. If your logic yields more than 7 races then please get back to us and we'll give you the solution of how 7 races should be sufficient to find 3 top horses from given 25 horses. You can then implement the logic in your program. Note there will be no points taken away from your programming skills for having asked for the solution to the puzzle.
	
	Do validate your program by sufficient number of tests that you can think of. Feel free to use test frameworks or any open source frameworks of your choice.
	
	Sample Input:
	Horses with randomly allocated speeds: H1 (10), H2(20), H3(45), H4(12), H5(57), H6(23) ... up to H25 (59)
	Sample Outputs:
	
	Races conducted:
	Race1 : H1, H2, H3, H4, H5 - Result (1st, 2nd, 3rd ranking horses) : H3, H2, H1
	Race2: H6, H7, H8, H9, .. - Result ...
	and so on...
	
	Final Race : H?, H?, H?, H?, H? - Final Result ( fastest 3 horses ...)
	
	Please note that the inputs/outputs shown above are representative, but as a gist, please do make sure that the output produced on the console indicates following items:
	1) List of horses with some sort of randomly allocated speeds (input)
	2) Number of total races conducted to find the top running 3 horses (output)
	3) List of horses that were running in each of the race that was conducted, in the order in which the races were conducted (output)
	4) Finally, outcome of each of the race, in form of top running horses (output)

Solution

	General
	- The solution to address the above problem is implemented in C# Dot Net
	- The solution is built with Visual Studio 2012, Dot Net Framework 4.5. Solution file: .\RaceSimulation\RaceSimulation.sln
	- The solution contains two projects
		- RaceSimulation
			- This project implements core logic as a library
		- RaceSimulationTest
			- This project is a Console Application. Main Class: RaceSimulationMain
			- This project also implements NUnit Tests on RaceSimulation
			
	Race Simulation (Runner <=> Thread)
	- In the Horse Race Simulation, a Thread represents a Runner running in a Race. The Race starts by forking of Multiple (ie. total 5) Threads and ends when the last Runner (ie. Thread) crosses the finishing line.
	- Random Runners are created programmatically with speeds between from 10 meter/second to 200 meter/second
	- Every Race is assumed to be of Distance 100 meters
	- Total 25 Runners are created and at a time 5 runners are allowed to Run in a single Race instance


	External Dependencies
	- Prism 4.1
		- The project leverages Prism 4.1 framework as it uses the UnityContainer for Dependency Injection and inversion of control.
		- These dependencies can be downloaded from: http://www.microsoft.com/en-in/download/details.aspx?id=28950
		- Alternatively, I have shared the dependency libraries: https://drive.google.com/folderview?id=0B2UB36w2uO9CV09YT2lmVGs4dnc&usp=sharing

	- NUnit
		- The test project has a dependency on NUnit framework 2.6.2.12296
		- NUnit dependency can be downloaded from my shared location: https://drive.google.com/folderview?id=0B2UB36w2uO9CV09YT2lmVGs4dnc&usp=sharing
		
	Unit Tests
	- TestRaceManager
		- Use NUnit to run a Random Horse Race. Assert on the Winners and make sure that the Winner, First Runner Up and Second Runner Up has got their speeds in decending order.
		
	- TestEliminationManager
		- Post running of the 7 expected Races, this unit test asserts that all the Runners except the top rankers are eliminated
		
	- TestRunnerManager
		- Tests creation of random Runners. Asserts on the ID and speed range of each runner
		
	- TestRace
		- Tests a Single Race instance. Asserts on the Results - making sure that the winners of the race have a higher speed than the loosers