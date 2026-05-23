package com.kezry.dacdx.config;

import android.app.Activity;
import android.os.AsyncTask;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.List;

public class MainActivity extends Activity {
    private final List<DeviceInfo> devices = new ArrayList<DeviceInfo>();
    private ArrayAdapter<DeviceInfo> adapter;
    private DeviceInfo selected;
    private EditText aliasEdit;
    private Spinner modeSpinner;
    private Spinner channelSpinner;
    private Spinner latencySpinner;
    private EditText delayEdit;
    private TextView selectedText;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        LinearLayout root = new LinearLayout(this);
        root.setOrientation(LinearLayout.VERTICAL);
        root.setPadding(20, 20, 20, 20);

        Button scan = new Button(this);
        scan.setText("Scan sound endpoints");
        root.addView(scan);

        ListView list = new ListView(this);
        adapter = new ArrayAdapter<DeviceInfo>(this, android.R.layout.simple_list_item_1, devices);
        list.setAdapter(adapter);
        root.addView(list, new LinearLayout.LayoutParams(-1, 0, 1));

        selectedText = new TextView(this);
        selectedText.setText("No device selected");
        root.addView(selectedText);

        aliasEdit = new EditText(this);
        aliasEdit.setHint("Alias");
        root.addView(aliasEdit);

        modeSpinner = spinner(new String[]{"sound_card", "dlna"});
        root.addView(modeSpinner);
        channelSpinner = spinner(new String[]{"front_left", "front_right", "front_center", "low_frequency", "rear_left", "rear_right", "side_left", "side_right"});
        root.addView(channelSpinner);
        latencySpinner = spinner(new String[]{"balanced", "fast", "stable"});
        root.addView(latencySpinner);

        delayEdit = new EditText(this);
        delayEdit.setHint("Manual delay ms");
        delayEdit.setText("0");
        root.addView(delayEdit);

        Button apply = new Button(this);
        apply.setText("Apply config");
        root.addView(apply);

        setContentView(root);

        scan.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                scanDevices();
            }
        });
        list.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                selected = devices.get(position);
                selectedText.setText("Selected: " + selected.alias + " / " + selected.deviceId);
                aliasEdit.setText(selected.alias);
            }
        });
        apply.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                applyConfig();
            }
        });
    }

    private Spinner spinner(String[] values) {
        Spinner spinner = new Spinner(this);
        ArrayAdapter<String> a = new ArrayAdapter<String>(this, android.R.layout.simple_spinner_item, values);
        a.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(a);
        return spinner;
    }

    private void scanDevices() {
        new AsyncTask<Void, Void, List<DeviceInfo>>() {
            @Override
            protected List<DeviceInfo> doInBackground(Void... params) {
                try {
                    return new DiscoveryClient().scan(2500);
                } catch (Exception e) {
                    return new ArrayList<DeviceInfo>();
                }
            }

            @Override
            protected void onPostExecute(List<DeviceInfo> result) {
                devices.clear();
                devices.addAll(result);
                adapter.notifyDataSetChanged();
                Toast.makeText(MainActivity.this, "Found " + result.size() + " endpoints", Toast.LENGTH_SHORT).show();
            }
        }.execute();
    }

    private void applyConfig() {
        if (selected == null) {
            Toast.makeText(this, "Select a device first", Toast.LENGTH_SHORT).show();
            return;
        }
        new AsyncTask<Void, Void, Boolean>() {
            @Override
            protected Boolean doInBackground(Void... params) {
                try {
                    int delay = Integer.parseInt(delayEdit.getText().toString());
                    return new ControlClient().configure(
                            selected,
                            aliasEdit.getText().toString(),
                            modeSpinner.getSelectedItem().toString(),
                            channelSpinner.getSelectedItem().toString(),
                            latencySpinner.getSelectedItem().toString(),
                            delay);
                } catch (Exception e) {
                    return false;
                }
            }

            @Override
            protected void onPostExecute(Boolean ok) {
                Toast.makeText(MainActivity.this, ok ? "Config applied" : "Config failed", Toast.LENGTH_SHORT).show();
            }
        }.execute();
    }
}
