using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;

namespace core.shareing
{
    public class Emailstringbody
    {
        public static string SendEmail(
     string email,
     string userId,
     string baseUrl,
     string code,
     string component,
     string subject,
     string message
 )
        {
            return $@"
<html> 
<head>
    <style>
        .button {{
            border: none;
            border-radius: 10px;
            padding: 15px 30px;
            color: #fff;
            display: inline-block;
            background: linear-gradient(45deg, #ff7e5f, #feb47b);
            cursor: pointer;
            text-decoration: none;
            box-shadow: 0 4px 15px rgba(0,0,0,0.2);
            transition: all 0.3s ease;
            font-size: 16px;
            font-weight: bold;
            font-family: 'Arial', sans-serif;
        }}
    </style>
</head>
<body>
    <h1>{component}</h1>
    <p>{message}</p>
    <p>Your verification code is: <strong>{code}</strong></p>
    <hr>
    <!-- لو عايز تسيب الزرار ممكن يكون مجرد رابط ثابت أو تعليمات -->
    <p>Enter this code in your app to verify your account.</p>
</body>
</html>";
        }
    }
}