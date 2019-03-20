using MachineCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MachineCalculator.Business
{
    public class Calculator
    {

        List<VM> _VMs = new List<VM>();
        List<double> loadList = new List<double>();
        public List<CalcAppData> _apps { get; set; }
        double totalCpu = 0;
        double totalRam = 0;
        double CpuLoad = 0;
        double RamLoad = 0;
        public int _VMAmmount { get; set; }

        public Calculator(List<CalcAppData> apps, int VMAmmount)
        {
            _VMAmmount = VMAmmount;
            _apps = apps;
            if (_apps.Max(a => a.instances) > _VMAmmount)
            {
                _VMAmmount = _apps.Max(a => a.instances);
            }
            int appscount = 0;
            foreach(CalcAppData current in apps)
            {
                if(current.flag == true)
                {
                    appscount += current.instances;
                }
            }
            if(appscount > _VMAmmount)
            {
                _VMAmmount = appscount;
            }
            for (int x = 0; x < _VMAmmount; x++)
            {
                _VMs.Add(new VM());
                foreach (AppParameters current in apps[0].resourses)
                {
                    _VMs[x].resourses.Add(new AppParameters(apps[0].resourses[apps[0].resourses.IndexOf(current)].name, 0));
                }
            }

            foreach (AppParameters current in _apps[0].resourses)
            {
                loadList.Add(0);
            }
        }

        public List<VM> CalculateHardware()
        {

            double totalRamLoad = 0;


            _apps = _apps.OrderByDescending(a => a.instances).ThenByDescending(a => a.RAM).ThenByDescending(a => a.CPU).ToList();
            foreach (CalcAppData current in _apps)
            {

                for (int x = 0; x < current.instances; x++)
                {
                    int writeAddress = int.MaxValue;
                    totalRamLoad += current.RAM;
                    //int minPowerDiff = int.MaxValue;
                    int zeroDiffAddress = int.MaxValue;
                    int minPowerAddress = int.MaxValue;
                    int minPower = int.MaxValue;
                    int CPUAddress = int.MaxValue;

                    _VMs = _VMs.OrderByDescending(v => v.ramLoad).ToList();
                    for (int y = 0; y < _VMs.Count(); y++)
                    {

                        if ((!_VMs[y].runningApps.Exists(m => m == current.name) && !(current.flag == true && _VMs[y].flaged == true)))
                        {
                            int power = 0;
                            int currentpower = 0;
                            while (_VMs[y].ramLoad > Math.Pow(2, currentpower))
                            {
                                currentpower++;
                            }
                            while (_VMs[y].ramLoad + current.RAM > Math.Pow(2, power))
                            {
                                power++;
                            }
                            if (minPower > power)
                            {
                                minPower = power;
                                minPowerAddress = y;
                            }
                            if (power - currentpower == 0)
                            {
                                zeroDiffAddress = y;
                            }
                            if (((_VMs[y].coreLoad + current.CPU) == Math.Truncate(_VMs[y].coreLoad + current.CPU) && power - currentpower == 0) || ((_VMs[y].coreLoad + current.CPU) % 2 == 0 && power - currentpower < 2))
                            {
                                CPUAddress = y;
                            }

                        }
                    }
                    if (CPUAddress != int.MaxValue)
                    {
                        writeAddress = CPUAddress;
                    }
                    else if (zeroDiffAddress != int.MaxValue)
                    {
                        writeAddress = zeroDiffAddress;
                    }
                    else
                    {
                        writeAddress = minPowerAddress;
                    }
                    if (current.flag == true)
                    {
                        _VMs[writeAddress].flaged = true;
                    }

                    foreach (AppParameters currentParam in current.resourses)
                    {
                        _VMs[writeAddress].resourses[current.resourses.IndexOf(currentParam)].load += currentParam.load;
                    }
                    _VMs[writeAddress].ramLoad += current.RAM;
                    _VMs[writeAddress].runningApps.Add(current.name);
                    _VMs[writeAddress].coreLoad += current.CPU;
                }

            }
            _VMs = _VMs.OrderBy(m => m.ramLoad).ToList();
            foreach (VM current in _VMs)
            {
                int CPU = 0;
                while (current.resourses.FirstOrDefault(m => m.name == "CPU_cores").load > CPU)
                {
                    CPU += 2;
                }
                current.coreLoad = CPU;
                int RAM = 0;
                while (current.resourses.FirstOrDefault(m => m.name == "RAM").load > Math.Pow(2, RAM))
                {
                    RAM++;
                }
                current.ramLoad = (int)Math.Pow(2, RAM);
            }
            return _VMs;
        }

        public List<VM> CalculateMedian()
        {
            foreach (CalcAppData current in _apps)
            {
                totalRam = totalRam + current.RAM * current.instances;
                totalCpu = totalCpu + current.CPU * current.instances;
            }


            _apps = _apps.OrderByDescending(a => a.RAM).ThenByDescending(a => a.instances).ThenByDescending(a => a.CPU).ToList();

            CpuLoad = 0;
            RamLoad = 0;

            foreach (CalcAppData current in _apps)
            {
                for (int x = 0; x < current.instances; x++)
                {
                    _VMs = _VMs.OrderBy(v => v.coreLoad).ThenBy(m => m.ramLoad).ToList();
                    foreach (AppParameters currentParam in current.resourses)
                    {
                        loadList[current.resourses.IndexOf(currentParam)] += currentParam.load;
                    }
                    int writeAddress = MinStdDeviationAddress(current);

                    if (current.flag == true) _VMs[writeAddress].flaged = true;
                    foreach (AppParameters currentParam in current.resourses)
                    {
                        _VMs[writeAddress].resourses[current.resourses.IndexOf(currentParam)].load += currentParam.load;
                    }
                    _VMs[writeAddress].runningApps.Add(current.name);
                }
            }
            _VMs = _VMs.OrderBy(m => m.ramLoad).ThenBy(m => m.coreLoad).ToList();
            return _VMs;
        }

        private int MinStdDeviationAddress(CalcAppData addedapp)
        {
            List<double> meanList = new List<double>();
            foreach (AppParameters current in addedapp.resourses)
            {
                meanList.Add(loadList[addedapp.resourses.IndexOf(current)] / _VMAmmount);
            }

            int minDivAddress = int.MaxValue;
            double minDiv = double.MaxValue;

            for (int x = 0; x < _VMs.Count(); x++)
            {

                List<double> summList = new List<double>();
                foreach (AppParameters current in addedapp.resourses)
                {
                    summList.Add(0);
                }

                if (!_VMs[x].runningApps.Exists(m => m == addedapp.name) && !(addedapp.flag == true && _VMs[x].flaged == true))
                {
                    for (int y = 0; y < _VMs.Count(); y++)
                    {
                        if (y == x)
                        {
                            //foreach (AppParameters currParam in addedapp.resourses)
                            //{
                                //int index = addedapp.resourses.IndexOf(currParam);
                                //summList[index] += Math.Pow(currParam.load + _VMs[y].resourses[index].load - meanList[index], 2);
                                summList[1] += Math.Pow(addedapp.resourses[1].load + _VMs[y].resourses[1].load - meanList[1], 2);
                                summList[5] += Math.Pow(addedapp.resourses[5].load + _VMs[y].resourses[5].load - meanList[5], 2);
                            //}
                        }
                        else
                        {
                            //foreach (AppParameters currParam in addedapp.resourses)
                            //{
                            //summList[addedapp.resourses.IndexOf(currParam)] += ( Math.Pow(_VMs[y].resourses[addedapp.resourses.IndexOf(currParam)].load - meanList[addedapp.resourses.IndexOf(currParam)], 2));
                            //}
                            summList[1] += (Math.Pow(_VMs[y].resourses[1].load - meanList[1], 2));
                            summList[5] += (Math.Pow(_VMs[y].resourses[5].load - meanList[5], 2));
                        }
                    }
                    List<double> divList = new List<double>();

                    //foreach (double current in summList)
                    //{
                    //if(!Double.IsNaN(meanList[summList.IndexOf(current)]))
                    //{
                    //    divList.Add(Math.Sqrt(current / _VMAmmount) * ((Math.Sqrt(current / _VMAmmount)) / _VMAmmount));
                    //}
                    // else
                    //{
                    //     divList.Add(Math.Sqrt(current / _VMAmmount));
                    //}

                    //}

                    if (!Double.IsNaN(meanList[1]))
                    {
                        divList.Add(Math.Sqrt(summList[1] / _VMAmmount) * ((Math.Sqrt(summList[1] / _VMAmmount)) / _VMAmmount));
                    }
                    else
                    {
                        divList.Add(Math.Sqrt(summList[1] / _VMAmmount));
                    }

                    if (!Double.IsNaN(meanList[5]))
                    {
                        divList.Add(Math.Sqrt(summList[5] / _VMAmmount) * ((Math.Sqrt(summList[5] / _VMAmmount)) / _VMAmmount));
                    }
                    else
                    {
                        divList.Add(Math.Sqrt(summList[5] / _VMAmmount));
                    }
                    double divSumm = 0;

                    foreach (double current in divList)
                    {
                        divSumm += current;
                    }
                    if (divSumm < minDiv)
                    {
                        minDiv = divSumm;
                        minDivAddress = x;
                    }
                }

            }
            return minDivAddress;
        }

    }
}
