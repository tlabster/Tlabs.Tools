# SmokeTester

An application **smoke test** is a basic check for the general possibility that an application is going to work.  
> (It is meant in the sense of *"electronic hardware testing"* where one would: Plug in a new board and turn on the power. If you see smoke coming from the board, turn off the power. No need to do any more testing...)

Thus the smoke test will check whether an application is going to startup properly by examination of the application's
console text output.

## Test Cases
The test cases for smoke testing an application are made of

* **Expectation test(s)**  
These are test(s) for specific lines of text that are required to appear in the console output of the application for a test to pass.  
(One test consists of one or multiple regular expression patterns used to be matched with a single output line.)  
If at least one of the test patterns is matching the output text line under test - that **expectation test**
is considered as passed and the smoke-testing continues with the next expectation test.
If a test without a next test defined succeeded, this is considered the ultimate test which causes the overall smoke-test to pass.  
(This especially is the assumed end of the application startup while the application of course would normally go on running...)  
Further **expectation test(s)** have
	* A name except for one test that must have an *empty* name, to indicate this is the default test that is applied as the first of all test(s).
	* A successor test (given by name) which is the next test to be applied once the current test has passed  
	-- except of the required ultimate test (having no successor).
	* An optional failure category to be applied to output lines that are tested once this test succeeded.

+ **Failure test(s)**  
Any output line that did not cause the current **expectation test** to succeed
is tested for lines that must **not** appear in the output at the current stage of the application startup.  
The set of failure test(s) applied to an output line is build by the union of global failure test(s)
(these must never appear over the entire tested output) and the failure test(s) of a category defined by the last
**expectation test** that was passed.  
Each **Failure test** pattern has a name to be optionally referred from **expectation test(s)** - except for global failure patterns (having an *empty* name).


## Test case definition
The test cases for an application's startup console output are defined in a text file whose name must be given with a command line parameter. The format of these test case definitions is:  
(Using the general convention that parts shown in [] brackets are optional.)

### Comment Line
Any line starting with optional whitespace followed by a number sign # is considered a comment line and is as such ignored.

### Empty Line
Lines having only whitespace are ignored as well.

### Command Line
`CMD-LINE`  
The command line to execute the application to be tested.

### Failure Test Definition:  
```? [FAIL-CATEGORY] : PATTERN```

* `FAIL-CATEGORY`
Optional failure test (category) name (must be prefixed with a question mark '?')
* `PATTERN`
Regular expression (starting immediately right from the colon ':') matched against an entire line of console text output.

Multiple **Failure Test Definition** with same `FAIL-CATEGORY` name forming a list of failure test `PATTERN`(s) to be referred with their common `FAIL-CATEGORY` name.

### Expectation Test Definition:  
```[TEST-NAME] [> SUCCESSOR-TEST] [? FAIL-CATEGORY] : PATTERN```  

* `TEST-NAME`  
Optional test name or *empty* name for initial default test
* `SUCCESSOR-TEST`  
Optional name of a successor test (must prefixed with '>')
* `FAIL-CATEGORY`  
Optional failure (category) name (must be prefixed with '?')

### Examples
The test case definition is illustrated best with some examples:

#### 1. Minimal test case
```
### Minimal test cases:
cmd.exe /?

:final-pattern
```

* This starts with a comment line followed by
	* The command to start the `cmd.exe` program (with parameter `/?`)
	* an empty line (ignored)
* Single expectation test
	* having no name, thus being the initial test
	* having no successor, thus also being the final test
	* defining the regular expression: `final-pattern` (that matches any output line containing the string `final-pattern`)

#### 2. Simple test case
```
### Simple test cases:
#
# Start process:
cmd.exe /?
#

?:default-fail-pattern
>final:start-pattern#1
final:final-pattern
```

* This starts with three comment lines followed by
	* The command to start the `cmd.exe` program (with parameter `/?`)
	* another comment and empty line
* A single global failure test (matching lines containing `default-fail-pattern`)
* A first expectation test (having no name)
	* with successor: `final`
* Final expectation test (with name `final`) and no successor
	* matching: `final-pattern`


#### 3. Basic test case
```
### Basic startup test cases:
#
# Start process:
cmd.exe /?
#
#Tests:


?:default-fail-pattern
>next?onFail:start-pattern#1
>next:start-pattern#2
next>next:next-pattern#1
next>final:next-pattern#2
final:final-pattern
```

* Has a command to start
* A single global failure test (matching lines containing `default-fail-pattern`)
* Two first expectation test(s) (having no name, both with successor: `next`)
	* #1 matching : `start-pattern#1`  
  and referring to a failure test `onFail` to be used together with the global failure test(s) if this test pattern would match  
	(NOTE: Since the failure test `onFail` is actualy not defined, this would silently result into an empty failure pattern)
	* #2 matching : `start-pattern#2`  
* Another two expectation test(s) (of name `next`)
	* #1 if matching : `next-pattern#1` proceeds testing with successor `next` (okay, that is the same, but allowed...)
	* #2 if matching : `next-pattern#2` proceeds testing with successor `final`
* Final expectation test (with name `final`) and no successor
	* matching: `final-pattern`


