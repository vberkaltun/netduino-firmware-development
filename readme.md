# intelliPWR

This repo gives information’s about intelliPWR project.  
You can find all related libraries and codes about intelliPWR from here.  
More info’s will be added later, stay tuned.

For now, you can find some examples about arduino on example branch.  
For Netduino side, this branch will be enough for you.

#### The Socket Pin Orders

The socket pin orders are given at below. This given orders must be same with your slave device.  
Otherwise, SCL will comm with SDA and SDA will comm with SCL.

|                    | Pin 1              | Pin 2              | Pin 3              | Pin 4              | Pin 5              |
| ------------------ | :----------------: | :----------------: | :----------------: | :----------------: | :----------------: |
| Pin                | D1                 | D1                 | GND                | D2                 | D2                 |
| Status             | SDA                | SDA                | GND                | SCL                | SCL                |
