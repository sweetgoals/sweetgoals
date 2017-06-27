package com.example.sweetgoals;

import java.io.FileOutputStream;
import java.io.IOException;
import java.io.FileInputStream;
import java.io.BufferedReader;
import java.io.InputStreamReader;

import org.ksoap2.SoapEnvelope;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.DialogInterface;
import android.graphics.Color;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.DatePicker;
import android.widget.Spinner;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

import android.content.Intent;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.AdapterView.OnItemClickListener;


public class goaldetail extends Activity{
	ListView lv;
	ArrayList<String> actDesc = new ArrayList<String>();
	ArrayAdapter lvAdaptor = null;
	String[] activityList;
	String goalNumber = "";
	String goalTitle = "";
    String user = "";
    String pass = "";
    String[] supportList;
    String timeLen = "";
    int i;
	protected CharSequence[] _options = null; // = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
	protected CharSequence[] daysAbrv = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
	protected boolean[] _selections = null; //  new boolean[ _options.length ];
    //Button supportButton = (Button) findViewById( R.id.supportButton);
	Button supportButton;

    @Override
	public void onCreate(Bundle savedInstanceState) {
    	String[] alist = null;
    	super.onCreate(savedInstanceState);
        setContentView(R.layout.goaldetail);       
        TextView gTitle = (TextView) findViewById(R.id.goalTitle);
        TextView gDueDate = (TextView) findViewById(R.id.completionDate);
        TextView gsDays = (TextView) findViewById(R.id.schDays);
        TextView timeLength = (TextView) findViewById(R.id.timeLength);
        lv = (ListView) findViewById(R.id.goalWorkLog);
        String goalDueDate = "";
        String sDays = "";
        

        Bundle extras = getIntent().getExtras();
        goalTitle = extras.getString("goalSelect");
        gTitle.setText("Goal Title: " + goalTitle);
        user = extras.getString("user");
        pass = extras.getString("pass");

        SoapObject request = new SoapObject("http://tempuri.org/", "getGoal");
        request.addProperty("user", user);
        request.addProperty("pass", pass);
        request.addProperty("goalTitle", goalTitle);
        SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
        envelope.setOutputSoapObject(request);
        envelope.dotNet = true;
        
        try {
        	HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
        	androidHttpTransport.call("http://tempuri.org/getGoal", envelope);
    		SoapObject result = (SoapObject)envelope.bodyIn;
    		goalNumber = result.getPropertyAsString(0);
            goalDueDate = result.getPropertyAsString(1);
            sDays = result.getPropertyAsString(2);
            timeLen = result.getPropertyAsString(3) + " " + result.getPropertyAsString(4);
        } catch (Exception e) {
        	e.printStackTrace();
        	Toast.makeText(getApplicationContext(), "in catch", Toast.LENGTH_LONG).show();
        }
        gDueDate.setText("Date Created: " + goalDueDate);
        gsDays.setText("Schedule Days: " + sDays);
        timeLength.setText("Time Length: " + timeLen);
    	TextView actText = (TextView) findViewById(R.id.hops);
    	
    	String[] splitDesc = null;
//        activityList = checkUser();   
//        alist = new String[activityList.length];
//        if (activityList[0].contains("No Activities"))
//        {
//        	actText.setText("# Activities: 0");
//        	alist[0] = "No Activities";
//        }
//        else 
//        {
//        	actText.setText("# Activities: " + activityList.length);      	
//        	for(i=0;i<alist.length;i++)
//        	{
//        		splitDesc = activityList[i].split("@");
//        		alist[i] = splitDesc[1];
//        	}
//        };
//
//        lvAdaptor = new ArrayAdapter<String>(this,android.R.layout.simple_list_item_1, alist);
//        lv.setAdapter(lvAdaptor);
//        lv.setTextFilterEnabled(true);
//        if (!activityList[0].contains("No Activities"))
//        {
//        	lv.setOnItemClickListener(new OnItemClickListener()
//        	{
//        		public void onItemClick(AdapterView<?> arg0, View v, int position, long id)
//        		{
//        			Intent myIntent = new Intent(getApplicationContext(), actdisplay.class);
//        			TextView gTitle = (TextView) findViewById(R.id.goalTitle);
//        			myIntent.putExtra("gNumber", goalNumber);                	
//        			myIntent.putExtra("gTitle", gTitle.getText().toString());
//        			myIntent.putExtra("user", user);
//        			myIntent.putExtra("pass", pass);
//        			myIntent.putExtra("actNumber", activityList[position].split("@")[0]);
////                	Toast.makeText(getApplicationContext(), "actNumber= " + activityList[position].split("@")[0], Toast.LENGTH_LONG).show();
//        			
//        			myIntent.putExtra("pos", position);
//        			myIntent.putExtra("timeLen", timeLen);
//        			startActivity(myIntent);
//        		}
//        	});               	        	
//        }
        
        Button addButton = (Button) findViewById(R.id.addWorkItem);
        addButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
            	Intent myIntent = new Intent(getApplicationContext(), actstart.class);
    			myIntent.putExtra("user", user);
    			myIntent.putExtra("pass", pass);
            	myIntent.putExtra("gNumber", goalNumber);
            	startActivity(myIntent);
            }     
        });   
        supportButton = (Button) findViewById( R.id.supportButton);       
        supportButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
                TextView gTitle = (TextView) findViewById(R.id.goalTitle);

            	Intent myIntent = new Intent(getApplicationContext(), supportgoal.class);
    			myIntent.putExtra("user", user);
    			myIntent.putExtra("pass", pass);
            	myIntent.putExtra("gNumber", goalNumber);
            	myIntent.putExtra("gTitle", goalTitle);
            	startActivity(myIntent);
            }     
        });   
    }
    
    @Override
    public void onResume()
    {      
    	super.onResume();
    	String[] alist = null;
    	activityList = checkUser();
    	String[] splitDesc;
    	int i;
        if (activityList.length >0)
        {
        	TextView actText = (TextView) findViewById(R.id.hops);
        	alist = new String[activityList.length];
            if (activityList[0].contains("No Activities"))
            {
            	actText.setText("# Activities: 0");
                alist[0] = "No Activities";
            }
            else 
            {
            	actText.setText("# Activities: " + activityList.length);              	
            	for(i=0;i<alist.length;i++)
            	{
            		splitDesc = activityList[i].split("@");
            		alist[i] = splitDesc[1];
            	}
            }
            if (!activityList[0].contains("No Activities"))
            {
            	lvAdaptor = null;
            	lvAdaptor = new ArrayAdapter<String>(this,android.R.layout.simple_list_item_1, alist);
            	lv.setAdapter(lvAdaptor);
            	lv.setTextFilterEnabled(true);           
            	lvAdaptor.notifyDataSetChanged();
            	lv.setOnItemClickListener(new OnItemClickListener()
            	{            		
            		public void onItemClick(AdapterView<?> arg0, View v, int position, long id)
            		{
            			Intent myIntent = new Intent(getApplicationContext(), actdisplay.class);
            			TextView gTitle = (TextView) findViewById(R.id.goalTitle);
            			myIntent.putExtra("gNumber", goalNumber);          			
            			myIntent.putExtra("actNumber", activityList[position].split("@")[0]);
//                    	Toast.makeText(getApplicationContext(), "actNumber= " + activityList[position].split("@")[0], Toast.LENGTH_LONG).show();
            			myIntent.putExtra("gTitle", gTitle.getText().toString());
            			myIntent.putExtra("user", user);
            			myIntent.putExtra("pass", pass);
            			myIntent.putExtra("pos", position);
            			myIntent.putExtra("timeLen", timeLen);
            			startActivity(myIntent);
            		}
            	});               	        	
            }
        }
    }
    
    public String[] checkUser() {
        String dataString = "";

        SoapObject request = new SoapObject("http://tempuri.org/", "listActivity");       
        request.addProperty("userName", user);
        request.addProperty("password", pass);
        request.addProperty("goalTitle", goalTitle);      
        
        SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
        envelope.setOutputSoapObject(request);
        envelope.dotNet = true;
        
        try {
        	HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
        	androidHttpTransport.call("http://tempuri.org/listActivity", envelope);
    		SoapObject result = (SoapObject)envelope.bodyIn;
    		dataString = result.getPropertyAsString(0);
        } catch (Exception e) {
        	e.printStackTrace();
        	Toast.makeText(getApplicationContext(), "Database Call Failed", Toast.LENGTH_LONG).show();
        }
        activityList = null;
        activityList = dataString.split("___");
        
       	return activityList;
    }
}
