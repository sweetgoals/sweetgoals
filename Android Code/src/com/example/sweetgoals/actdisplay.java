package com.example.sweetgoals;

import org.ksoap2.SoapEnvelope;
import org.ksoap2.SoapFault;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapPrimitive;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;
import org.kxml2.kdom.Document;
import org.xml.sax.InputSource;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Interpolator.Result;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;
import android.widget.AdapterView.OnItemClickListener;

import java.util.*;
import java.io.StringReader;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.text.DateFormat;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

public class actdisplay extends Activity{
    String goalTitleStr = "";
    String stopTimeStr = "";
    String[] values;
    String[] picList = {"No Pictures"};
    String aNumber = "";
    String timeLength = "";
    ListView lv;
    
	ArrayAdapter lvAdaptor = null;
	
	String rawValues = "";
	

//    values cells:
//    0. actTitle
//    1. actDesc
//    2. good
//    3. bad
//    4. supportConfirmed
//    5. actDate
//    6. startTime
//    7. stopTime
//    8. timeDiff
//    9. userName
    
    String user = "";
    String pass = "";
    String time = "";
    
	@Override
	public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.actdisplay);
	}
	
    @Override
    public void onResume()
    {      
    	super.onResume();
    	display();
    }
    
    public void display()
    {
    	TextView goalTitle = (TextView) findViewById(R.id.goalTitle);
        TextView actTitle = (TextView) findViewById(R.id.actTitle);
        TextView actNumber = (TextView) findViewById(R.id.actNumber);
        TextView actDate = (TextView) findViewById(R.id.actDate);
        TextView actDesc = (TextView) findViewById(R.id.actDesc);
        TextView actGood = (TextView) findViewById(R.id.actGood);
        TextView actBad = (TextView) findViewById(R.id.actBad);
        TextView actVerify = (TextView) findViewById(R.id.actVerify);
        TextView startTime = (TextView) findViewById(R.id.startTime);
        TextView stopTime = (TextView) findViewById(R.id.stopTime);
        TextView actTime = (TextView) findViewById(R.id.actTime);
        TextView tarTime = (TextView) findViewById(R.id.tarTime);

        String startTimeStr = "";
        String stopTimeStr = "";

    	Button stopButton = (Button) findViewById(R.id.stopButton);
    	Button verButton = (Button) findViewById(R.id.verList);
        stopButton.setVisibility(View.GONE);
               
        int goalNumber = 0;
        int position = 0;
        String picDesc[] = null;
        
        Bundle extras = getIntent().getExtras();
	    if (extras != null)
	    {
	     goalNumber = Integer.parseInt(extras.getString("gNumber"));
	     aNumber = extras.getString("actNumber");
	     goalTitleStr = extras.getString("gTitle");
	     user = extras.getString("user");
	     pass = extras.getString("pass");
	     position = extras.getInt("pos") + 1;
	     timeLength = extras.getString("timeLen");
	    }

        verButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
    			Intent myIntent = new Intent(getApplicationContext(), viewresponse.class);
    			myIntent.putExtra("user", user);
    			myIntent.putExtra("pass", pass);
    			myIntent.putExtra("actNumber", aNumber);
    			startActivity(myIntent);
            }
        });

        SoapObject request = new SoapObject("http://tempuri.org/", "activitySummary");
		request.addProperty("userName", user);
		request.addProperty("password", pass);
		request.addProperty("actNumber", aNumber);	
//		Toast.makeText(getApplicationContext(), "userName=" + user, Toast.LENGTH_LONG).show();
//		Toast.makeText(getApplicationContext(), "password=" + pass, Toast.LENGTH_LONG).show();
//		Toast.makeText(getApplicationContext(), "actNumber=" + aNumber, Toast.LENGTH_LONG).show();
    	
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
		envelope.setOutputSoapObject(request);
		envelope.dotNet = true;
		try {
			HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
			androidHttpTransport.call("http://tempuri.org/activitySummary", envelope);
			SoapObject result = (SoapObject)envelope.bodyIn;
			goalTitle.setText(goalTitleStr);
			actTitle.setText("Activity Title" + result.getPropertyAsString(1));
			Log.d("values=", result.getPropertyAsString(0));
			
			rawValues = result.getPropertyAsString(0);
//			values = result.getPropertyAsString(0).split("___");
			
//			Toast.makeText(getApplicationContext(), "string 0=" + result.getPropertyAsString(0), Toast.LENGTH_LONG).show();
//			Toast.makeText(getApplicationContext(), "string 1=" + result.getPropertyAsString(1), Toast.LENGTH_LONG).show();
			if (result.getPropertyAsString(1)!= "")
				picList = result.getPropertyAsString(1).split("___"); 
			else			
			{
				picList = new String[1];
				picList[0] = "No Pictures";
			}
			
		} catch (Exception e) {
			e.printStackTrace();
			String str= ((SoapFault) envelope.bodyIn).faultstring;
			Toast.makeText(getApplicationContext(),  str, Toast.LENGTH_LONG).show();
			Log.d("soap error", "str " + str);
		}
		

		
//		Toast.makeText(getApplicationContext(), "stop= " + stopTimeStr, Toast.LENGTH_LONG).show();
		actTitle.setText("Activity Title: " + getValue(rawValues, "<actTitle>", "</actTitle>"));
		actNumber.setText("Activity Number: " + aNumber);
		actDate.setText("Date:" + getValue(rawValues, "<actDate>", "</actDate>"));

//		Toast.makeText(getApplicationContext(), "date= " + values[5], Toast.LENGTH_LONG).show();

		actDesc.setText("Description: " + getValue(rawValues, "<actDesc>", "</actDesc>"));
        startTimeStr = getValue(rawValues, "<startTime>", "</startTime>");

        startTime.setText("Start Time: " + startTimeStr);
        stopTimeStr = getValue(rawValues, "<stopTime>", "</stopTime>");
		if (stopTimeStr.length()==0)
		{
//    		Toast.makeText(getApplicationContext(), "in onclick", Toast.LENGTH_LONG).show();			
			stopButton.setVisibility(View.VISIBLE);
	        stopButton.setOnClickListener(new View.OnClickListener() {
	            public void onClick(View view) {
//	        		Toast.makeText(getApplicationContext(), "in onclick", Toast.LENGTH_LONG).show();

	            	SimpleDateFormat sdf = new SimpleDateFormat("MM/dd/yyyy");
	        	    sdf = new SimpleDateFormat("HH:mm");
	        	    time = sdf.format(new Date());
	        	    String stopTime;
	        	    stopTime = time.toString();
	        	    //Toast.makeText(getApplicationContext(), "stopTime= " + stopTime, Toast.LENGTH_LONG).show();	        	    
	        	    //start time
	        	    int time1 = Integer.parseInt(getValue(rawValues, "<startTime>", "</startTime>").substring(0,2))*60;
	        	    time1 += Integer.parseInt(getValue(rawValues, "<startTime>", "</startTime>").substring(3));
	        	    
	        	    // end time
	        	    int time2 = Integer.parseInt(stopTime.substring(0,2))*60;
	        	    time2 += Integer.parseInt(stopTime.substring(3));
	        	    int hDiff, mDiff;
	        	    
	        	    if (time2<time1)
	        	    {
	        	    	hDiff = ((1440 - time1) + time2) / 60;  
	        	    	mDiff = ((1440 - time1) + time2) % 60;
	        	    }
	        	    else
	        	    {
		        	    hDiff = (time2 - time1) / 60;
                        mDiff = (time2 - time1) % 60;
	        	    }
//	        	    Toast.makeText(getApplicationContext(), "Hdiff= " + hDiff, Toast.LENGTH_LONG).show();
//	        	    Toast.makeText(getApplicationContext(), "Mdiff= " + mDiff, Toast.LENGTH_LONG).show();
	        	    StringBuilder builder = new StringBuilder();
	        	    if(hDiff < 10)
	        	     builder.append("0");
	        	    builder.append(hDiff);
	        	    builder.append(":");
	        	    if(mDiff < 10)
	        	     builder.append("0");
	        	    builder.append(mDiff);
	        	    String timeDiff = builder.toString();
//	        		Toast.makeText(getApplicationContext(), "startTime= " + values[6], Toast.LENGTH_LONG).show();
//	        		Toast.makeText(getApplicationContext(), "stopTime=" + stopTime, Toast.LENGTH_LONG).show();
//	        		Toast.makeText(getApplicationContext(), "tiemDiff " + timeDiff, Toast.LENGTH_LONG).show();	        	   
                	Intent myIntent = new Intent(getApplicationContext(), actstop.class);
                	myIntent.putExtra("actNum", aNumber);
                	myIntent.putExtra("gTitle", goalTitleStr);                	
                	myIntent.putExtra("actTitle", getValue(rawValues, "<actTitle>", "</actTitle>"));
                	myIntent.putExtra("startTime", getValue(rawValues, "<startTime>", "</startTime>"));
                	myIntent.putExtra("stopTime", stopTime);
                	myIntent.putExtra("duration", timeDiff);
                	myIntent.putExtra("user", user);
                	myIntent.putExtra("pass", pass);                	
	        	    startActivity(myIntent);
	            }     
	        });      
		}
		else
		{
//			Toast.makeText(getApplicationContext(), "in else", Toast.LENGTH_LONG).show();
			try
			{
				actGood.setText("Good News: " + getValue(rawValues, "<good>", "</good>"));
				actBad.setText("Bad News: " + getValue(rawValues, "<bad>", "</bad>"));
				actVerify.setText("Verified: " + getValue(rawValues, "<supportConfirmed>", "</supportConfirmed>"));
				startTime.setText("Start Time: " + getValue(rawValues, "<startTime>", "</startTime>"));
				stopTime.setText("Stop Time: " + getValue(rawValues, "<stopTime>", "</stopTime>"));
				actTime.setText("Time Spent: " + getValue(rawValues, "<timeDiff>", "</timeDiff>"));
				tarTime.setText("Target Time: " + timeLength);
				lv = (ListView) findViewById(R.id.picListView);
		        lv.setAdapter(null);
		        picDesc = new String[picList.length];
		        int i =0;
		        
//		        Toast.makeText(getApplicationContext(), "picList length " + picList.length, Toast.LENGTH_LONG).show();
//		        Toast.makeText(getApplicationContext(), "picList " + picList[0], Toast.LENGTH_LONG).show();
				if (!picList[0].contains("No Pictures"))
				{
					for(i=0;i<picList.length;i++)
					{
						picDesc[i] = picList[i].split("@")[1];
					}
				}
				else			
				{
					picDesc = new String[1];
					picDesc[0] = "No Pictures";
				}	
//				Toast.makeText(getApplicationContext(), "picList= " + picList[0], Toast.LENGTH_LONG).show();	        	   
		        lvAdaptor = new ArrayAdapter<String>(this,android.R.layout.simple_list_item_1, picDesc);            
		        lv.setAdapter(lvAdaptor);
		        lv.setTextFilterEnabled(true);           
		        lvAdaptor.notifyDataSetChanged();
	        	lv.setOnItemClickListener(new OnItemClickListener() {
	        		public void onItemClick(AdapterView<?> arg0, View v, int position, long id)
	        		{
	        			Intent myIntent = new Intent(getApplicationContext(), pictureview.class);
	        			myIntent.putExtra("user", user);
	        			myIntent.putExtra("pass", pass);
	        			myIntent.putExtra("pos", position);
	        			myIntent.putExtra("picNumber", picList[position].split("@")[0]);
	        			startActivity(myIntent);
	        		}
	        	});               	        	
//			Toast.makeText(getApplicationContext(), "stop time Not blank", Toast.LENGTH_LONG).show();
			} catch (Exception e) {
				e.printStackTrace();
				Toast.makeText(getApplicationContext(), "in catch", Toast.LENGTH_LONG).show();
			}			
		}   	
    }
    
    public String getValue(String fString, String fTag, String bTag)
    {
		String parseValue = "";
		Integer front = 0;
		Integer back = 0;
		try
		{
			front = fString.indexOf(fTag) + fTag.length();
			back = fString.indexOf(bTag);
			parseValue = fString.substring(front, back);
//			Log.d("fString", fString);
//			Log.d("front", front.toString());
//			Log.d("back", back.toString());
//			Log.d("parsevalue", parseValue);
//			Toast.makeText(getApplicationContext(), "stoptimeParse= " + parseValue, Toast.LENGTH_LONG).show();
			return parseValue;
		}catch (Exception e) {
			Log.d("error", e.toString());
			return "";
		}
    }
}
