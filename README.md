# Parallel-encryption

A C# program that uses the Caesar cipher to encrypt a given text 5000 times.  
'Encryption.cs' in ##code is where the script is run from. Before runtime, make sure to have the input text file in the same folder. The input files can be found in the ##information folder.  

-Performance is measured by these results:  
-Time taken with 6 threads: 39.3853ms  
-Time taken with 5 threads: 43.8273ms  
-Time taken with 4 threads: 51.2707ms  
-Time taken with 3 threads: 57.0708ms  
-Time taken with 2 threads: 71.9311ms  
-Time taken with 1 threads: 110.1972ms  

-At which point we reach the same efficiency level as the serial code.  
-Time taken without threads: 112.3665ms  
