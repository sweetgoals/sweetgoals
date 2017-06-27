package com.example.sweetgoals;

import java.io.FileOutputStream;
import java.io.IOException;

import org.ksoap2.SoapEnvelope;
import org.ksoap2.SoapFault;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;

import android.app.Activity;
import android.content.Context;
import android.graphics.Color;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

public class createuser extends Activity{
    
	@Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.createuser);
        Button userButton = (Button) findViewById(R.id.createButton);
        Button cancelButton = (Button) findViewById(R.id.cancelButton);
        
        
        userButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
                EditText userText = (EditText) findViewById(R.id.goalDesc);
                EditText passText = (EditText) findViewById(R.id.editText2);
                EditText passVerifyText = (EditText) findViewById(R.id.editText3);
                EditText emailText = (EditText) findViewById(R.id.editText4);
                EditText emailVerifyText = (EditText) findViewById(R.id.emailVerifyText2);
                
                String passTextStr = passText.getText().toString();
                String passVerifyTextStr = passVerifyText.getText().toString();
                boolean createAccount = false;
                boolean vuser = false;
                boolean vemail = false;
                
            	HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");

            	SoapObject request = new SoapObject("http://tempuri.org/", "checkUserAvail");
                request.addProperty("user", userText.getText().toString());
                SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
                envelope.setOutputSoapObject(request);
                envelope.dotNet = true;
                try {
                	androidHttpTransport.call("http://tempuri.org/checkUserAvail", envelope);
            		SoapObject result = (SoapObject)envelope.bodyIn;                    
                    vuser = Boolean.parseBoolean(result.getPropertyAsString(0));
            		if (vuser==true)
            		{
            			userText.setBackgroundColor(Color.GREEN);
            			createAccount=true;
            		}
            		else 
            		{
            			userText.setBackgroundColor(Color.RED);
                    	Toast.makeText(getApplicationContext(), "User Unavailable", Toast.LENGTH_LONG).show();
                    	createAccount = false;                    	
            		}               		
                } catch (Exception e) {
        			e.printStackTrace();
        			String error = ((SoapFault) envelope.bodyIn).faultstring;
        			Toast.makeText(getApplicationContext(),  error, Toast.LENGTH_LONG).show();
        			Log.d("soap error", "checkUserAvail error= " + error);
                }

            		if (!passTextStr.equals(passVerifyTextStr)) 
                	{
                    	Toast.makeText(getApplicationContext(), "Password Mismatch", Toast.LENGTH_LONG).show();
                		passVerifyText.setBackgroundColor(Color.RED);
                		createAccount = false;
                	}
            		
                	if (!emailText.getText().toString().equals(emailVerifyText.getText().toString())) 
                	{
                		Toast.makeText(getApplicationContext(), "Email Mismatch", Toast.LENGTH_LONG).show();
                		emailVerifyText.setBackgroundColor(Color.RED);
                		createAccount = false;
                	}
                	else 
                	{
                    	request = new SoapObject("http://tempuri.org/", "checkUserEmail");
                        request.addProperty("email", emailText.getText().toString());
                        envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
                        envelope.setOutputSoapObject(request);
                        envelope.dotNet = true;
                        try {
                        	androidHttpTransport.call("http://tempuri.org/checkUserEmail", envelope);
                    		SoapObject result = (SoapObject)envelope.bodyIn;                    
                            vemail = Boolean.parseBoolean(result.getPropertyAsString(0));
                    		if (vemail==true)
                    		{
                    			emailText.setBackgroundColor(Color.GREEN);
                    			createAccount=true;
                    		}
                    		else 
                    		{
                    			emailText.setBackgroundColor(Color.RED);
                            	Toast.makeText(getApplicationContext(), "Email Unavailable", Toast.LENGTH_LONG).show();
                            	createAccount = false;                    	
                    		}               		
                        } catch (Exception e) {
                			e.printStackTrace();
                			String error = ((SoapFault) envelope.bodyIn).faultstring;
                			Toast.makeText(getApplicationContext(),  error, Toast.LENGTH_LONG).show();
                			Log.d("soap error", "checkUserEmail error= " + error);
                        }
                	}
                if (createAccount==true)
                {
                	request = new SoapObject("http://tempuri.org/", "createAccount");
                    request.addProperty("user", userText.getText().toString());
                    envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
                    envelope.setOutputSoapObject(request);
                    envelope.dotNet = true;
                    try {
                        request.addProperty("username", userText.getText().toString());
                        request.addProperty("password", passText.getText().toString());
                        request.addProperty("email", emailText.getText().toString());

                    	//HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
                    	androidHttpTransport.call("http://tempuri.org/createAccount", envelope);
                		SoapObject result = (SoapObject)envelope.bodyIn;                       
                        vuser = Boolean.parseBoolean(result.getPropertyAsString(0));
                        if (vuser ==false)
                         Toast.makeText(getApplicationContext(), "User not Created", Toast.LENGTH_LONG).show();
                    } catch (Exception e) {
            			e.printStackTrace();
            			String error = ((SoapFault) envelope.bodyIn).faultstring;
            			Toast.makeText(getApplicationContext(),  error, Toast.LENGTH_LONG).show();
            			Log.d("soap error", "createAccount error= " + error);
                    }
                    if(vuser==true)
                    {
                    	try {          			
                    		FileOutputStream fos = openFileOutput("personalFile".toString(), Context.MODE_PRIVATE);
                    		fos.write((userText.getText().toString() + "\n").getBytes());
                    		fos.write((passText.getText().toString() + "\n").getBytes());
                    		fos.write((emailText.getText().toString()).getBytes());
                    		fos.close();
                    		finish();        			
                    	} catch (IOException e) {
                			e.printStackTrace();
                			String error = "";
                			Toast.makeText(getApplicationContext(),  error, Toast.LENGTH_LONG).show();
                			Log.d("soap error", "Write File Error= " + error);
                    	}
                    }
                }                		
            }            	        	
        });      	
        
        cancelButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
            	finish();
            }            	        	
        });      	
	};
}
