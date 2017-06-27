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
import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.DatePicker;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import java.util.Calendar;
import java.util.Date;


public class creategoalsimple extends Activity{

	protected CharSequence[] _options = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
	protected CharSequence[] daysAbrv = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
	protected boolean[] _selections =  new boolean[ _options.length ];
	
	Button scheduleDaysButton,saveButton,cancelButton;
	EditText goalTitle, timeLength;
	DatePicker goalFinishDate;
	Spinner timeUnit;
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.creategoalsimple);
        
        scheduleDaysButton = ( Button ) findViewById( R.id.scheduleDays );
        scheduleDaysButton.setOnClickListener( new scheduleDaysClick());
        
        cancelButton = (Button) findViewById(R.id.cancelButton);
        cancelButton.setOnClickListener(new cancelClick());
        
        saveButton = (Button) findViewById(R.id.saveButton);
        saveButton.setOnClickListener(new saveClick());
        
	};

	public class scheduleDaysClick implements View.OnClickListener {
		public void onClick( View view ) {
			showDialog( 0 );
		}
	}

	public class cancelClick implements View.OnClickListener {
		public void onClick( View view ) {
			finish();
		}
	}

	public class saveClick implements View.OnClickListener {
		String user, pass;

		public void onClick( View view ) {
	        goalTitle = (EditText) findViewById(R.id.goalTitleText);
	        timeLength = (EditText) findViewById(R.id.timeLength);
	        goalFinishDate = (DatePicker) findViewById(R.id.completeDate);
	        timeUnit = (Spinner) findViewById(R.id.timeUnit);
	        
	        String goalTitleStr = goalTitle.getText().toString();
	        String timeLengthStr = timeLength.getText().toString();
	        
	        Integer aday = goalFinishDate.getDayOfMonth();
	        Integer amonth = goalFinishDate.getMonth() + 1;
	        Integer ayear = goalFinishDate.getYear();        
	        String goalFinishDateStr =  amonth.toString() + "/" + aday.toString() + "/" + ayear.toString();

	        String timeUnitStr = timeUnit.getSelectedItem().toString();
	        String scheduleDaysStr = scheduleDaysButton.getText().toString();
	        
	        Integer i;
	        Calendar todayDate = Calendar.getInstance();
	        Calendar userDate = Calendar.getInstance();
	        todayDate.add(Calendar.DAY_OF_YEAR, -1);
	        userDate.set(goalFinishDate.getYear(), goalFinishDate.getMonth(), goalFinishDate.getDayOfMonth(), 0, 0, 0);
	        if (!(goalTitle.getText().toString().equals("") ||
  	        	  timeLength.getText().toString().equals("") ||
	        	 userDate.before(todayDate) ||
	        	 (scheduleDaysButton.getText().equals("") ||
	        	  scheduleDaysButton.getText().equals("Select Days"))))
	        {
	        	userDate.toString();
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
	            if(checkUser(user,pass) == true)
	            {
	            	createGoal(user, pass, goalTitleStr, goalFinishDateStr, scheduleDaysStr, timeLengthStr,timeUnitStr);
	            }
	            else Toast.makeText(getApplicationContext(), "Bad User", Toast.LENGTH_LONG).show();	          
	        }
	        else {
	        	Toast.makeText(getApplicationContext(), "Bad Goal Inputs", Toast.LENGTH_LONG).show();
	        }		
		}
				
	    public boolean checkUser(String user, String pass) {
	  	
	        SoapObject request = new SoapObject("http://tempuri.org/", "verifyUserPass");
	        request.addProperty("user", user);
	        request.addProperty("pass", pass);
	        SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
	        envelope.setOutputSoapObject(request);
	        envelope.dotNet = true;
	        boolean vUser = false;
	        try {
	        	HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
	        	androidHttpTransport.call("http://tempuri.org/verifyUserPass", envelope);
	    		SoapObject result = (SoapObject)envelope.bodyIn;
	            vUser = Boolean.parseBoolean(result.getPropertyAsString(0));
	        } catch (Exception e) {
	        	e.printStackTrace();
	        	Toast.makeText(getApplicationContext(), "Unable to Verify User", Toast.LENGTH_LONG).show();
	        }
            return vUser;
	    }

	    public void createGoal(String user, String pass, String title, String cDate, String sDays, String tLength, String uLength) {	  	
 	        String sqlResult = "";
   	        EditText goalDesc = (EditText) findViewById(R.id.goalDesc);
	        
	    	SoapObject request = new SoapObject("http://tempuri.org/", "createGoal");
	        request.addProperty("userName", user);
	        request.addProperty("password", pass);
	        request.addProperty("goalTitle", title);
	        request.addProperty("goalDueDate", cDate);
	        request.addProperty("scheduleDays", sDays);
	        request.addProperty("timeLength", tLength);
	        request.addProperty("timeUnit", uLength);
	        request.addProperty("goalDesc", goalDesc.getText().toString());
	        
	        SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
	        envelope.setOutputSoapObject(request);
	        envelope.dotNet = true;	        
	        try {
	        	HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
	        	androidHttpTransport.call("http://tempuri.org/createGoal", envelope);
	    		SoapObject result = (SoapObject)envelope.bodyIn;
	            sqlResult = result.getPropertyAsString(0);            
	        } catch (Exception e) {
	        	e.printStackTrace();
	        	Toast.makeText(getApplicationContext(), "Database Call Failed", Toast.LENGTH_LONG).show();
	        }

	        if (sqlResult.contains("Goal Created"))
	        {
	        	Toast.makeText(getApplicationContext(), sqlResult, Toast.LENGTH_LONG).show();
	        	finish();
	        }
			else 
				Toast.makeText(getApplicationContext(), sqlResult, Toast.LENGTH_LONG).show();
	    }
				
	        
	}

	@Override
	protected Dialog onCreateDialog( int id ) 
	{
		return 
		new AlertDialog.Builder( this )
        	.setTitle( "Select Days" )
        	.setMultiChoiceItems( _options, _selections, new DialogSelectionClickHandler() )
        	.setPositiveButton( "OK", new DialogButtonClickHandler() )
        	.create();
	}
	
	
	public class DialogSelectionClickHandler implements DialogInterface.OnMultiChoiceClickListener
	{
		public void onClick( DialogInterface dialog, int clicked, boolean selected )
		{
			printSelectedDays();
		}
	}
	

	public class DialogButtonClickHandler implements DialogInterface.OnClickListener
	{
		public void onClick( DialogInterface dialog, int clicked )
		{
			switch( clicked )
			{
				case DialogInterface.BUTTON_POSITIVE:
					printSelectedDays();
					break;
			}
		}
	}
	
	protected void printSelectedDays(){
		String dispAbr = "";
		
		for( int i = 0; i < _options.length; i++ ){
			if (_selections[i]==true)
				dispAbr += daysAbrv[i]+",";		
		}
		scheduleDaysButton.setText(dispAbr);
	}

}


