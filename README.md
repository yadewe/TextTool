# TextTool
Text string format tool. It can read your clipboard text data to format, and then write back to clipboard.
## Format like this:
### Case 1 add comma
#### raw data:
```235693
6119978
3279656
434811
3226945
313292
100427
```
#### formated data:
```
235693,6119978,3279656,434811,3226945,313292,100427
```

### Case 2 add comma and single quote
#### raw data:
```
235693
6119978
3279656
434811
3226945
313292
100427
```
#### formated data:
```
'235693','6119978','3279656','434811','3226945','313292','100427'
```
## More usage please type the cmd
```
TextTool.exe ?
```
## example
```
TextTool type=1 count=50 sepa=, pre=' suf=' count=20 item_reg="[a-zA-Z0-9\.+_-]{1,}" tip=0 rep=0
```