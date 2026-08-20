# Mail Traffic Analyzer

A system for email transmission log analysis, hourly server inactivity tracking, and minimal-sender analytics built with C# and ASP.NET.

---

## Program User Manual

### 1. Data Preparation
Prepare the input plain-text (`.txt`) files according to two distinct categories:

* **Server Speed Configuration File:**  
  Contains server connection and processing speeds.  
  * **Naming Requirement:** The file name **must** contain the keyword `greit` or `speed` (e.g., `serveriai_greitis.txt`).  
  * **Format:** Each line contains the server name and its bandwidth:  
    `<Server_Name> <Speed_Bytes_Per_Sec>`  
  * *Example:*
    ```text
    mail.server1.com 102400
    mail.server2.com 204800
    ```

* **Server Activity Log Files:**  
  Log files detailing daily email transmissions.  
  * **Naming Requirement:** File names **must not** contain the words `greit` or `speed`.  
  * **Structure:**  
    * **Line 1:** Server name and date: `<Server_Name> <Date>`  
    * **Subsequent Lines:** Space or tab-separated transmission records:  
      `<Time> <Sender_Email> <Receiver_Email> <Size_Bytes>`  
  * *Example:*
    ```text
    mail.server1.com 2026-05-10
    08:15:30 sender1@domain.com receiver1@domain.com 512000
    08:45:00 sender2@domain.com receiver2@domain.com 1048576
    ```

---

### 2. How to Use

1. **Launch the Application:** Run the solution in Visual Studio (`F5` or `Ctrl + F5`).
2. **Upload Data Files:** Use the multi-file upload control to select all prepared speed and log files simultaneously, then click the data upload button. The system validates the format and populates initial overview tables with server speeds and transmission logs.
3. **Analyze Server Inactivity:** Click the inactivity analysis button to calculate transmission durations (accounting for multi-hour transfers) and identify all hours where a server transmitted zero bytes.
4. **Identify Lowest-Volume Senders:** Select a specific date from the dropdown list and click the minimum-sender search button to find which sender(s) sent the fewest emails on that day.
