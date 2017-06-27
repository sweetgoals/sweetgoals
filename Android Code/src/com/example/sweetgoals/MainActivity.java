package com.example.sweetgoals;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;

import org.ksoap2.SoapEnvelope;
import org.ksoap2.SoapFault;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.app.ListActivity;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;
import android.widget.AdapterView.OnItemClickListener;
import android.app.AlertDialog;

public class MainActivity extends Activity {
	ListView lv;
	String[] goalsList = {"Add Some Goals :-)"};
	ArrayAdapter lvAdaptor = null;
	String user = "";
	String pass = "";
	
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        Button logButton = (Button) findViewById(R.id.loginButton);       
        Button supportButton = (Button) findViewById(R.id.supportButton);
        Button createGoalButton = (Button) findViewById(R.id.creategoal);       

        lv = (ListView) findViewById(R.id.goalList);
        lvAdaptor = new ArrayAdapter<String>(this,android.R.layout.simple_list_item_1, goalsList);
        lv.setAdapter(lvAdaptor);
        lv.setTextFilterEnabled(true);

        logButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
            	Intent myIntent = new Intent(view.getContext(), login.class);
            	myIntent.putExtra("frommain", "1");
                startActivityForResult(myIntent, 0);
            }     
        });      
        createGoalButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
            	Intent myIntent = new Intent(view.getContext(), creategoalsimple.class);
            	myIntent.putExtra("frommain", "1");
                startActivityForResult(myIntent, 0);
            }     
        });    
        supportButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
            	Intent myIntent = new Intent(view.getContext(), supporters.class);
            	myIntent.putExtra("user", user);
            	myIntent.putExtra("pass", pass);
                startActivityForResult(myIntent, 0);
            }     
        });    
    }

    @Override
    public void onResume()
    {      
    	super.onResume();

    	goalsList = checkUser();
        lv.setAdapter(null);
        lvAdaptor = new ArrayAdapter<String>(this,android.R.layout.simple_list_item_1, goalsList);            
        lv.setAdapter(lvAdaptor);
        lv.setTextFilterEnabled(true);           
        lvAdaptor.notifyDataSetChanged();
        lv.setOnItemClickListener(new OnItemClickListener()
       	{
           	public void onItemClick(AdapterView<?> arg0, View v, int position, long id)
           	{
           		Intent myIntent = new Intent(v.getContext(), goaldetail.class);
           		myIntent.putExtra("goalSelect", lv.getItemAtPosition(position).toString());
           		myIntent.putExtra("user", user);
           		myIntent.putExtra("pass", pass);
           		startActivityForResult(myIntent, 0);
           	}
       	});
    }
    
    public String[] checkUser() {
        Integer i;
        user = "";
        pass = "";
        String[] goals = {""};
        String dataString = "";
        try
    	{
            FileInputStream in = openFileInput("personalFile");
    		InputStreamReader inputStreamReader = new InputStreamReader(in);
    		BufferedReader bufferedReader = new BufferedReader(inputStreamReader);
    		String line;
    		i=0;
    		while ((line = bufferedReader.readLine()) != null) {
    			if((i==0) && (line!=""))
   	    			user =line;
    			else if((i==1) && (line!=""))
   	    			pass = line;
    			i++;
    		}
    		in.close();
    	} catch (IOException e) {
    		e.printStackTrace();
    	}

        SoapObject request = new SoapObject("http://tempuri.org/", "goalSummary");
        request.addProperty("user", user);
        request.addProperty("pass", pass);
        SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);
        envelope.setOutputSoapObject(request);
        envelope.dotNet = true;

        try {
            HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
            androidHttpTransport.call("http://tempuri.org/goalSummary", envelope);
            SoapObject result = (SoapObject)envelope.bodyIn;
            dataString = result.getPropertyAsString(0);
        } catch (Exception e) {
            e.printStackTrace();
                String error = ((SoapFault) envelope.bodyIn).faultstring;
                Toast.makeText(getApplicationContext(),  error, Toast.LENGTH_LONG).show();
                Log.d("soap error", "error= " + error);
        }
        goals = dataString.split("___");
        if (goals.length>0)
        	return goals;
        else return new String[]{"Invalid User"};
    }
}
