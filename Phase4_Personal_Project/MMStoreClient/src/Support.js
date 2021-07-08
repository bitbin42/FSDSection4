//import React from "react";

export const DateText = (date,format) => {
  const months=['January','February','March','April','May','June','July','August','September','October','November','December']
  const dt=new Date(date);
  let x=dt.getDate();
  let fdt=format.replace('d',x);
  fdt=fdt.replace('yyyy',dt.getFullYear());
  fdt=fdt.replace('MMMM',months[dt.getMonth()]);
  fdt=fdt.replace('MMM',months[dt.getMonth()].substring(0,3));
  let hr=dt.getHours();
  fdt=fdt.replace('H',hr);
  x=(hr<12)?"AM":"PM";
  fdt=fdt.replace('@',x);
  if (hr>12) hr-=12;
  fdt=fdt.replace('h',hr);
  x=dt.getMinutes();
  if (x===0) x=12;
  if (x<10) x='0'+x;
  fdt=fdt.replace('mm',x);
  return fdt;
};

