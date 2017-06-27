package com.example.sweetgoals;

import org.ksoap2.SoapEnvelope;
import org.ksoap2.SoapFault;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;
import android.widget.EditText;

public class actstop extends Activity{
	String user, pass, actTitle, gTitle, startTime, stopTime, duration, actNum, good, bad;

	@Override
	public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.actstop);
    	user = "";
    	pass = "";
    	actTitle = "";
    	gTitle = "";
    	startTime = "";
    	stopTime = "";
    	duration = "";
    	actNum = "";
    	good = "";
    	bad = "";

        Bundle extras = getIntent().getExtras();
	    if (extras != null)
	    {	    	    	
	    	user = extras.getString("user");
	    	pass = extras.getString("pass");
	    	gTitle = extras.getString("gTitle");
	    	actTitle = extras.getString("actTitle");
	    	startTime = extras.getString("startTime");
	    	stopTime = extras.getString("stopTime");
	    	duration = extras.getString("duration");
	    	actNum = extras.getString("actNum");
	    }

	    TextView goalTitleText = (TextView) findViewById(R.id.goalTitle);
	    TextView aTitle = (TextView) findViewById(R.id.actTitle);
	    TextView staTime = (TextView) findViewById(R.id.startTime);
	    TextView stoTime = (TextView) findViewById(R.id.stopTime);
	    TextView durTime = (TextView) findViewById(R.id.durTime);
	    goalTitleText.setText(gTitle);
	    aTitle.setText("Activity Title: " + actTitle);
	    staTime.setText("Start Time: " + startTime);
	    stoTime.setText("Stop Time: " + stopTime);
	    durTime.setText("Duration: " + duration);	    

	    Button sendVer = (Button) findViewById(R.id.sendVer);
	    Button takePicture = (Button) findViewById(R.id.button1);
	    
	    takePicture.setOnClickListener(new View.OnClickListener() {
	    	public void onClick(View view)
	    	{
            	Intent myIntent = new Intent(view.getContext(), pictures.class);
            	myIntent.putExtra("user", user);
            	myIntent.putExtra("pass", pass);
            	myIntent.putExtra("actNum", actNum);
                startActivityForResult(myIntent, 0);
	    	}
	    });
	    
        sendVer.setOnClickListener(new View.OnClickListener() {
        	public void onClick(View view) {
        		EditText eText1 = (EditText) findViewById(R.id.goalDesc);
        		EditText eText2 = (EditText) findViewById(R.id.editText2);
        		
        		good = eText1.getText().toString();
        		bad = eText2.getText().toString();

//    			Toast.makeText(getApplicationContext(),  "user= " + user, Toast.LENGTH_LONG).show();
//    			Toast.makeText(getApplicationContext(),  "password= " + pass, Toast.LENGTH_LONG).show();
//    			Toast.makeText(getApplicationContext(),  "actNumber= " + actNum, Toast.LENGTH_LONG).show();
//    			Toast.makeText(getApplicationContext(),  "good= " + eText1.getText().toString(), Toast.LENGTH_LONG).show();
//    			Toast.makeText(getApplicationContext(),  "bad= " + eText2.getText().toString(), Toast.LENGTH_LONG).show();
//    			Toast.makeText(getApplicationContext(),  "stoptime= " + stopTime, Toast.LENGTH_LONG).show();
//    			Toast.makeText(getApplicationContext(),  "timeDiff= " + duration, Toast.LENGTH_LONG).show();

        		SoapObject request = new SoapObject("http://tempuri.org/", "finishActivity");
	       		request.addProperty("userName", user);
	       		request.addProperty("password", pass);
	       		request.addProperty("actNumber", actNum);
	       		request.addProperty("good", eText1.getText().toString());
	       		request.addProperty("bad", eText2.getText().toString());
	       		request.addProperty("stopTime", stopTime);
	       		request.addProperty("timeDiff", duration);       

	       		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
	       		envelope.setOutputSoapObject(request);  
	       		envelope.dotNet = true;	             
	       		try {
	       			HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
	       			androidHttpTransport.call("http://tempuri.org/finishActivity", envelope);
	       			//SoapObject result = (SoapObject)envelope.bodyIn;
	       			//Toast.makeText(getApplicationContext(), result.getPropertyAsString(0), Toast.LENGTH_LONG).show();
	       		} catch (Exception e) {
	    			e.printStackTrace();
	    			String str= ((SoapFault) envelope.bodyIn).faultstring;
	    			Toast.makeText(getApplicationContext(),  str, Toast.LENGTH_LONG).show();
	    			Log.d("soap error 1", "str " + str);
	       		}
	       		finish();
	        }     
        });      
	};
}
