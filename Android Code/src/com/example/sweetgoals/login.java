package com.example.sweetgoals;

import java.io.FileOutputStream;
import java.io.IOException;
import java.io.FileInputStream;
import java.io.BufferedReader;
import java.io.InputStreamReader;

import org.ksoap2.SoapEnvelope;
import org.ksoap2.SoapFault;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

public class login extends Activity {
	int justEditInfo = 0;
	
    @Override
	public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.login);

        Button save = (Button) findViewById(R.id.submitButton);
        Button createUser = (Button) findViewById(R.id.createButton);
        SoapObject result;
        int i = 0;
        EditText loginText, passText, emailText;
        loginText = (EditText) findViewById(R.id.loginText);
    	passText = (EditText) findViewById(R.id.passwordText);
    	emailText = (EditText) findViewById(R.id.emailText);

        try
    	{
            FileInputStream in = openFileInput("personalFile");
    		InputStreamReader inputStreamReader = new InputStreamReader(in);
    		BufferedReader bufferedReader = new BufferedReader(inputStreamReader);
    		String line;
    		i=0;
    		while ((line = bufferedReader.readLine()) != null) {
    			if((i==0) && (line!=""))
   	    			loginText.setText(line);
    			else if((i==1) && (line!=""))
   	    			passText.setText(line);
    			else if((i==2) && (line!=""))
   	    			emailText.setText(line);
    			i++;
    		}
    		in.close();
    	} catch (IOException e) {
    		e.printStackTrace();
    	}
        checkUser();

        save.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
           		EditText nameN=(EditText)findViewById(R.id.loginText);          	
          		EditText pass=(EditText)findViewById(R.id.passwordText);
          		EditText emaiL=(EditText)findViewById(R.id.emailText);

           		try {          			
           			FileOutputStream fos = openFileOutput("personalFile".toString(), Context.MODE_PRIVATE);
           			fos.write((nameN.getText().toString() + "\n").getBytes());
           			fos.write((pass.getText().toString() + "\n").getBytes());
          			fos.write((emaiL.getText().toString()).getBytes());
          			fos.close();
           		} catch (IOException e) {
           			e.printStackTrace();
           		}
           		if (checkUser() != -1)
         			finish();
            }     
        }); 
        
		Button cButton = (Button) findViewById(R.id.clearButton);
        cButton.setOnClickListener(new View.OnClickListener() {
        	public void onClick(View view) {
           		EditText nameN=(EditText)findViewById(R.id.loginText);          	
          		EditText emaiL=(EditText)findViewById(R.id.emailText);
          		EditText pass=(EditText)findViewById(R.id.passwordText);

          		nameN.setText("");
          		emaiL.setText("");
          		pass.setText("");
            	nameN.setBackgroundColor(Color.WHITE);
            	emaiL.setBackgroundColor(Color.WHITE);
            	pass.setBackgroundColor(Color.WHITE);
        	}     
        });  

        createUser.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
            	Intent myIntent = new Intent(view.getContext(), createuser.class);
            	myIntent.putExtra("frommain", "1");
                startActivityForResult(myIntent, 0);
            }     
        });      
    }   

    public int checkUser() {
        EditText loginText, passText, emailText;
        loginText = (EditText) findViewById(R.id.loginText);
    	passText = (EditText) findViewById(R.id.passwordText);
    	emailText = (EditText) findViewById(R.id.emailText);
  	    SoapObject result;
        boolean vuser=false;
        boolean vpass=false;
        boolean vemail=false;
        SoapObject request = new SoapObject("http://tempuri.org/", "checkUser");
        request.addProperty("user", loginText.getText().toString());
        request.addProperty("pass", passText.getText().toString());
        request.addProperty("email", emailText.getText().toString());
        SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
        envelope.setOutputSoapObject(request);
        envelope.dotNet = true;
        try {
        	HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
        	androidHttpTransport.call("http://tempuri.org/checkUser", envelope);
    		result = (SoapObject)envelope.bodyIn;
            vuser = Boolean.parseBoolean(result.getPropertyAsString(0));
            vpass = Boolean.parseBoolean(result.getPropertyAsString(1));
            vemail = Boolean.parseBoolean(result.getPropertyAsString(2));

            if (vuser==true)
            {
                loginText.setBackgroundColor(Color.GREEN);
                if (vpass==true)
                {
                    passText.setBackgroundColor(Color.GREEN);
                    if (vemail==true)
                        emailText.setBackgroundColor(Color.GREEN);
                    else
                    {
                        emailText.setBackgroundColor(Color.RED);
                        Toast.makeText(getApplicationContext(), "Bad Email", Toast.LENGTH_LONG).show();
                    }
                }
                else {
                    passText.setBackgroundColor(Color.RED);
                    Toast.makeText(getApplicationContext(), "Bad Password", Toast.LENGTH_LONG).show();
                    return -1;
                }
            }
            else{
                loginText.setBackgroundColor(Color.RED);
                Toast.makeText(getApplicationContext(), "Bad Username", Toast.LENGTH_LONG).show();
                return -1;
            }
        } catch (Exception e) {
			e.printStackTrace();
            Toast.makeText(getApplicationContext(), "In catch Login", Toast.LENGTH_LONG).show();
			String error = ((SoapFault) envelope.bodyIn).faultstring;
			Toast.makeText(getApplicationContext(),  error, Toast.LENGTH_LONG).show();
			Log.d("soap error", "error= " + error);
        }
        return 0;
    }
};

