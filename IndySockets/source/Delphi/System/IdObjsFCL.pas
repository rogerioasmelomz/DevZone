{
  $Project$
  $Workfile$
  $Revision$
  $DateUTC$
  $Id$

  This file is part of the Indy (Internet Direct) project, and is offered
  under the dual-licensing agreement described on the Indy website.
  (http://www.indyproject.org/)

  Copyright:
   (c) 1993-2005, Chad Z. Hower and the Indy Pit Crew. All rights reserved.
}
{
  $Log$
}

unit IdObjsFCL;

interface

uses
  System.Collections, System.Collections.Specialized, System.Text, System.IO,
  System.Threading, IdException, System.ComponentModel;

type             
  TIdStringListFCL = class;
  TIdNetList = class;
  TIdNetSeekOrigin = (soBeginning, soCurrent, soEnd);
  EReadError = Exception;
  EWriteError = Exception;
  TByteArray = array of Byte;
  TIdNetPersistent = MarshalByRefObject;
  TIdNetNativeComponent = class;
  TIdNetPersistentHelper = class helper for TIdNetPersistent
  protected
    procedure AssignTo(Dest: TIdNetPersistent); virtual;
    function GetOwner: TIdNetPersistent; virtual;
    procedure InsideCreate; virtual;
  public
    constructor Create; overload; virtual;
    constructor Create(AOwner: TIdNetNativeComponent); overload; virtual;
    procedure Assign(ASource: TIdNetPersistent); virtual;
    function GetNamePath: string; virtual;
  end;
  
  TIdNetNativeComponentState = set of (csLoading, csDesigning, csDestroying,
        csFreeNotification);
  TIdNetNativeOperation = (opInsert, opRemove);
  TIdNetNativeComponentSite = class;
  TIdNetComponentName = string;
  TIdNetNativeComponent = class(Component, ISupportInitialize)
  private
    FFreeNotifies: TIdNetList;
    FComponents: TIdNetList;
    FComponentState: TIdNetNativeComponentState;
    FOwner: TIdNetNativeComponent;
    procedure RemoveNotification(AComponent: TIdNetNativeComponent);
  protected
    function GetComponentState: TIdNetNativeComponentState; virtual;
    procedure BeginInit;
    procedure EndInit;
    procedure Loaded; virtual;
    function GetOwner: TIdNetPersistent; override;
    procedure InsideCreate; override;
    procedure Notification(AComponent: TIdNetNativeComponent;
      Operation: TIdNetNativeOperation); virtual;
    function GetSiteObject: TIdNetNativeComponentSite;
    function GetName: string;
    function GetTag: &Object;
    procedure SetName(const Value: TIdNetComponentName); virtual;
    procedure SetTag(const Value: &Object);
    function GetSelfOwner: TIdNetNativeComponent;
  public
    procedure FreeNotification(AComponent: TIdNetNativeComponent);
    procedure RemoveFreeNotification(AComponent: TIdNetNativeComponent);
    property ComponentState: TIdNetNativeComponentState read GetComponentState;
    property Owner: TIdNetNativeComponent read GetSelfOwner;
  published
    property Name: string read GetName write SetName stored False;
    property Tag: &Object read GetTag write SetTag;
  end;

  TIdNetNativeComponentHelper = class helper(TIdNetPersistentHelper) for TIdNetNativeComponent
  public
    constructor Create(AOwner: TIdNetNativeComponent); reintroduce; virtual;
  end;

  TIdNetNativeComponentSite = class(&Object, ISite)
  private
    FComponent: IComponent;
    FOwner: TIdNetNativeComponent;
    FName: string;
    FTag: &Object;
    FDesignMode: Boolean;
  protected
  public
    property Component: IComponent read FComponent;
    function get_Container: IContainer;
    property Container: IContainer read get_Container;
    property DesignMode: Boolean read FDesignMode;
    property Name: string read FName write FName;

    function GetService(AType: System.Type): &Object;
    constructor Create(AInstance, AOwner: TIdNetNativeComponent); reintroduce;
  end;

  TIdNetMultiReadExclusiveWriteSynchronizer = class
  private
    FReaderWriterLock: System.Threading.ReaderWriterLock;
    function GetRevisionLevel: Integer;
  public
    constructor Create;

    procedure BeginRead;
    procedure EndRead;
    function BeginWrite: Boolean;
    procedure EndWrite;

    property RevisionLevel: Integer read GetRevisionLevel;
  end;

  TIdNetStream = class
  private
    function Skip(Amount: Integer): Integer;
    procedure SetPosition(const Pos: Int64);
  protected
    function GetPosition: Int64; virtual; abstract;
    function GetSize: Int64; virtual; abstract;
    procedure SetSize(NewSize: Int64); overload; virtual; abstract;
  public
    function Read(var Buffer: array of Byte; Offset, Count: Longint): Longint; overload; virtual; abstract;
    function Read(var Buffer: array of Byte; Count: Longint): Longint; overload;
    function Read(var Buffer: Byte): Longint; overload;
    function Read(var Buffer: Byte; Count: Longint): Longint; overload; 
    function Read(var Buffer: Boolean): Longint; overload;
    function Read(var Buffer: Boolean; Count: Longint): Longint; overload; 
    function Read(var Buffer: Char): Longint; overload;
    function Read(var Buffer: Char; Count: Longint): Longint; overload; 
    function Read(var Buffer: AnsiChar): Longint; overload;
    function Read(var Buffer: AnsiChar; Count: Longint): Longint; overload; 
    function Read(var Buffer: ShortInt): Longint; overload;
    function Read(var Buffer: ShortInt; Count: Longint): Longint; overload; 
    function Read(var Buffer: SmallInt): Longint; overload;
    function Read(var Buffer: SmallInt; Count: Longint): Longint; overload; 
    function Read(var Buffer: Word): Longint; overload;
    function Read(var Buffer: Word; Count: Longint): Longint; overload; 
    function Read(var Buffer: Integer): Longint; overload;
    function Read(var Buffer: Integer; Count: Longint): Longint; overload; 
    function Read(var Buffer: Cardinal): Longint; overload;
    function Read(var Buffer: Cardinal; Count: Longint): Longint; overload;
    function Read(var Buffer: Int64): Longint; overload;
    function Read(var Buffer: Int64; Count: Longint): Longint; overload; 
    function Read(var Buffer: UInt64): Longint; overload;
    function Read(var Buffer: UInt64; Count: Longint): Longint; overload; 
    function Read(var Buffer: Single): Longint; overload;
    function Read(var Buffer: Single; Count: Longint): Longint; overload; 
    function Read(var Buffer: Double): Longint; overload;
    function Read(var Buffer: Double; Count: Longint): Longint; overload; 
    function Read(var Buffer: Extended): Longint; overload;
    function Read(var Buffer: Extended; Count: Longint): Longint; overload; 
    function Write(const Buffer: array of Byte; Offset, Count: Longint): Longint; overload; virtual; abstract;
    function Write(const Buffer: array of Byte; Count: Longint): Longint; overload;
    //function Write(const Buffer: Byte): Longint; overload;
    //function Write(const Buffer: Byte; Count: Longint): Longint; overload;
    function Write(const Buffer: Boolean): Longint; overload;
    function Write(const Buffer: Boolean; Count: Longint): Longint; overload;
    function Write(const Buffer: Char): Longint; overload;
    function Write(const Buffer: Char; Count: Longint): Longint; overload;
    function Write(const Buffer: AnsiChar): Longint; overload;
    function Write(const Buffer: AnsiChar; Count: Longint): Longint; overload;
    function Write(const Buffer: ShortInt): Longint; overload;
    function Write(const Buffer: ShortInt; Count: Longint): Longint; overload;
    function Write(const Buffer: SmallInt): Longint; overload;
    function Write(const Buffer: SmallInt; Count: Longint): Longint; overload;
    function Write(const Buffer: Word): Longint; overload;
    function Write(const Buffer: Word; Count: Longint): Longint; overload;
    function Write(const Buffer: Integer): Longint; overload;
    function Write(const Buffer: Integer; Count: Longint): Longint; overload;
    function Write(const Buffer: Cardinal): Longint; overload;
    function Write(const Buffer: Cardinal; Count: Longint): Longint; overload;
    function Write(const Buffer: Int64): Longint; overload;
    function Write(const Buffer: Int64; Count: Longint): Longint; overload;
    function Write(const Buffer: UInt64): Longint; overload;
    function Write(const Buffer: UInt64; Count: Longint): Longint; overload;
    function Write(const Buffer: Single): Longint; overload;
    function Write(const Buffer: Single; Count: Longint): Longint; overload;
    function Write(const Buffer: Double): Longint; overload;
    function Write(const Buffer: Double; Count: Longint): Longint; overload;
    function Write(const Buffer: Extended): Longint; overload;
    function Write(const Buffer: Extended; Count: Longint): Longint; overload;
    function Seek(const Offset: Int64; Origin: TIdNetSeekOrigin): Int64; overload; virtual; abstract;
    procedure ReadBuffer(Buffer: array of Byte; Count: Longint); overload;
    procedure ReadBuffer(var Buffer: Byte); overload;
    procedure ReadBuffer(var Buffer: Byte; Count: Longint); overload;
    procedure ReadBuffer(var Buffer: Boolean); overload;
    procedure ReadBuffer(var Buffer: Boolean; Count: Longint); overload;
    procedure ReadBuffer(var Buffer: Char); overload;
    procedure ReadBuffer(var Buffer: Char; Count: Longint); overload;
    procedure ReadBuffer(var Buffer: AnsiChar); overload;
    procedure ReadBuffer(var Buffer: AnsiChar; Count: Longint); overload;
    procedure ReadBuffer(var Buffer: ShortInt); overload;
    procedure ReadBuffer(var Buffer: ShortInt; Count: Longint); overload;
    procedure ReadBuffer(var Buffer: SmallInt); overload;
    procedure ReadBuffer(var Buffer: SmallInt; Count: Longint); overload;
    procedure ReadBuffer(var Buffer: Word); overload;
    procedure ReadBuffer(var Buffer: Word; Count: Longint); overload;
    procedure ReadBuffer(var Buffer: Integer); overload;
    procedure ReadBuffer(var Buffer: Integer; Count: Longint); overload;
    procedure ReadBuffer(var Buffer: Cardinal); overload;
    procedure ReadBuffer(var Buffer: Cardinal; Count: Longint); overload;
    procedure ReadBuffer(var Buffer: Int64); overload;
    procedure ReadBuffer(var Buffer: Int64; Count: Longint); overload;
    procedure ReadBuffer(var Buffer: UInt64); overload;
    procedure ReadBuffer(var Buffer: UInt64; Count: Longint); overload;
    procedure ReadBuffer(var Buffer: Single); overload;
    procedure ReadBuffer(var Buffer: Single; Count: Longint); overload;
    procedure ReadBuffer(var Buffer: Double); overload;
    procedure ReadBuffer(var Buffer: Double; Count: Longint); overload;
    procedure ReadBuffer(var Buffer: Extended); overload;
    procedure ReadBuffer(var Buffer: Extended; Count: Longint); overload;
    procedure WriteBuffer(const Buffer: array of Byte; Count: Longint); overload;
    //procedure WriteBuffer(const Buffer: Byte); overload;
    //procedure WriteBuffer(const Buffer: Byte; Count: Longint); overload;
    procedure WriteBuffer(const Buffer: Boolean); overload;
    procedure WriteBuffer(const Buffer: Boolean; Count: Longint); overload;
    procedure WriteBuffer(const Buffer: Char); overload;
    procedure WriteBuffer(const Buffer: Char; Count: Longint); overload;
    procedure WriteBuffer(const Buffer: AnsiChar); overload;
    procedure WriteBuffer(const Buffer: AnsiChar; Count: Longint); overload;
    procedure WriteBuffer(const Buffer: ShortInt); overload;
    procedure WriteBuffer(const Buffer: ShortInt; Count: Longint); overload;
    procedure WriteBuffer(const Buffer: SmallInt); overload;
    procedure WriteBuffer(const Buffer: SmallInt; Count: Longint); overload;
    procedure WriteBuffer(const Buffer: Word); overload;
    procedure WriteBuffer(const Buffer: Word; Count: Longint); overload;
    procedure WriteBuffer(const Buffer: Integer); overload;
    procedure WriteBuffer(const Buffer: Integer; Count: Longint); overload;
    procedure WriteBuffer(const Buffer: Cardinal); overload;
    procedure WriteBuffer(const Buffer: Cardinal; Count: Longint); overload; 
    procedure WriteBuffer(const Buffer: Int64); overload;
    procedure WriteBuffer(const Buffer: Int64; Count: Integer); overload; 
    procedure WriteBuffer(const Buffer: UInt64); overload;
    procedure WriteBuffer(const Buffer: UInt64; Count: Integer); overload; 
    procedure WriteBuffer(const Buffer: Single); overload;
    procedure WriteBuffer(const Buffer: Single; Count: Integer); overload; 
    procedure WriteBuffer(const Buffer: Double); overload;
    procedure WriteBuffer(const Buffer: Double; Count: Integer); overload;
    procedure WriteBuffer(const Buffer: Extended); overload;
    procedure WriteBuffer(const Buffer: Extended; Count: Integer); overload;
    function CopyFrom(Source: TIdNetStream; Count: Int64): Int64;
    class operator Implicit(const Value: TIdNetStream): System.IO.Stream;
    class operator Implicit(const Value: System.IO.Stream): TIdNetStream;
    property Position: Int64 read GetPosition write SetPosition;
    property Size: Int64 read GetSize write SetSize;
  end;

  TIdNetCLRStreamWrapper = class(TIdNetStream)
  protected
    FHandle: System.IO.Stream;
    procedure SetSize(AValue: Int64); override;
    function GetPosition: Int64; override;
    function GetSize: Int64; override;
  public
    constructor Create(AHandle: System.IO.Stream);
    destructor Destroy; override;
    function Read(var Buffer: array of Byte; Offset, Count: Longint): Longint; override;
    function Write(const Buffer: array of Byte; Offset, Count: Longint): Longint; override;
    function Seek(const Offset: Int64; Origin: TIdNetSeekOrigin): Int64; override;
    property Handle: System.IO.Stream read FHandle;
  end;

  TIdNetWrapperFCLStream = class(System.IO.Stream)
  protected
    FStream: System.IO.Stream;
  public
    constructor Create(Stream: TIdNetStream); overload;
    { overridden methods of System.IO.Stream }
    procedure Close; override;
    procedure Flush; override;
    function get_CanRead: Boolean; override;
    function get_CanSeek: Boolean; override;
    function get_CanWrite: Boolean; override;
    function get_Length: Int64; override;
    function get_Position: Int64; override;
    function Read(Buffer: array of Byte; Offset: Integer; Count: Integer): Integer; override;
    function Seek(Offset: Int64; Origin: System.IO.SeekOrigin): Int64; override;
    procedure SetLength(Value: Int64); override;
    procedure set_Position(Value: Int64); override;
    procedure Write(Buffer: array of Byte; Offset: Integer; Count: Integer); override;
    property CanRead: Boolean read get_CanRead;
    property CanSeek: Boolean read get_CanSeek;
    property CanWrite: Boolean read get_CanWrite;
    property Length: Int64 read get_Length;
    property Position: Int64 read get_Position write set_Position;
  public
    destructor Destroy; override;
    class function GetStream(Stream: TIdNetStream): System.IO.Stream; static;
  end;

  TIdNetMemoryStream = class(TIdNetStream)
  private
    FFCLStream: System.IO.MemoryStream;
  protected
    function GetPosition: Int64; override;
    function GetSize: Int64; override;
    procedure SetSize(AValue: Int64); override;
    function GetMemory: TByteArray;
  public
    constructor Create; virtual;
    destructor Destroy; override;

    function Read(var Buffer: array of Byte; Offset, Count: Longint): Longint; overload; override;
    function Write(const Buffer: array of Byte; Offset, Count: Longint): Longint; overload; override;
    function Seek(const Offset: Int64; Origin: TIdNetSeekOrigin): Int64; overload; override;
    property Memory: TByteArray read GetMemory;
  end;

  TIdNetStringStream = class(TIdNetMemoryStream)
  private
  protected
    function GetString: string;
  public
    constructor Create(const AString: string); reintroduce; overload;
    procedure WriteString(const AString: string);
    property DataString: string read GetString;
  end;

  TIdNetFileStream = class(TIdNetStream)
  strict private
    FFCLStream: System.IO.FileStream;
  protected
    function GetPosition: Int64; override;
    function GetSize: Int64; override;
    procedure SetSize(AValue: Int64); override;
  public
    constructor Create(const AFileName: string; const AMode: UInt16); reintroduce; overload;
    constructor Create(const AFileName: string; const AMode: UInt16; const ARight: Cardinal); reintroduce; overload;
    destructor Destroy; override;

    function Read(var Buffer: array of Byte; Offset, Count: Longint): Longint; overload; override;
    function Write(const Buffer: array of Byte; Offset, Count: Longint): Longint; overload; override;
    function Seek(const Offset: Int64; Origin: TIdNetSeekOrigin): Int64; overload; override;
  end;

  TIdStringListSortCompareFCL = function (AList: TIdStringListFCL; AIndex1, AIndex2: Integer) : Integer;
  TIdDuplicates = (dupIgnore, dupAccept, dupError);
  EIdStringListErrorFCL = class(EIdException);
  TIdStringsDefined = set of (sdDelimiter, sdQuoteChar, sdNameValueSeparator);

  TIdStringsFCL = class(TIdNetPersistent, ICloneable, IEnumerable, ICollection)
  private
    FDefined: TIdStringsDefined;
    FDelimiter: Char;
    FQuoteChar: Char;
    FNameValueSeparator: Char;
    FUpdateCount: Integer;
    function GetCommaText: string;
    function AnsiQuotedStr(AValue: string; AQuote: string): string;
    function GetDelimitedText: string;
    function GetName(AIndex: Integer): string;
    function GetValue(AName: string): string;
    procedure SetCommaText(AValue: string);
    function AnsiExtractQuotedStr(AValue: string; AQuote: Char) : string;
    procedure SetDelimitedText(AValue: string);
    procedure SetValue(AName, AValue: string);
    function GetDelimiter: Char;
    procedure SetDelimiter(AValue: Char);
    function GetQuoteChar: Char;
    procedure SetQuoteChar(AValue: Char);
    function GetNameValueSeparator: Char;
    procedure SetNameValueSeparator(AValue: Char);
    function GetValueFromIndex(AIndex: Integer): string;
    procedure SetValueFromIndex(AIndex: Integer; AValue: string);
  protected
    function ExtractName(S: string): string;
    function GetCapacity: Integer; virtual;
    function GetCount: Integer; virtual; abstract;
    function GetObject(AIndex: Integer) : &Object; virtual;
    function GetTextStr: string; virtual;
    procedure PutObject(AIndex: Integer; AObject: &Object); virtual;
    procedure SetCapacity(ACapacity: Integer); virtual;
    procedure SetTextStr(AText: string); virtual;
    procedure SetUpdateState(AUpdating: Boolean); virtual;
    function Get(AIndex: Integer): string; overload; virtual; abstract;
    procedure Put(AIndex: Integer; AValue: string); overload; virtual;
    function CompareStrings(S1, S2: string): Integer; virtual;
    function GetIsSynchronized: Boolean;
    function GetSyncRoot: &Object;
    function GetIsFixedSize: Boolean;
    function GetIsReadOnly: Boolean;
    property UpdateCount: Integer read FUpdateCount;
  public
    constructor Create;

    class operator Implicit(const aValue: TIdStringsFCL): StringCollection;
    class operator Implicit(AValue: StringCollection): TIdStringsFCL;

    function Clone: &Object; virtual;
    function GetEnumerator: IEnumerator;
    procedure CopyTo(ADest: &Array; AIndex: Integer); virtual; abstract;
    function Add(AObject: &Object): Integer; overload;
    function Contains(AObject: &Object): Boolean;
    procedure Insert(AIndex: Integer; AObject: &Object); overload;
    function IndexOf(AObject: &Object): Integer; overload;
    procedure Remove(AObject: &Object);
    procedure RemoveAt(AIndex: Integer);
    function ToString: string; override;

    procedure SaveToStream(AStream: TIdNetStream);
    procedure SaveToFile(AFileName: string);
    procedure LoadFromStream(AStream: TIdNetStream);
    procedure LoadFromFile(AFileName: string);

    function Add(const S: string): Integer; overload; virtual;
    function AddObject(S: string; AObject: &Object) : Integer; virtual;
    procedure Append(S: string);
    procedure AddStrings(AStrings: TIdStringsFCL); virtual;
    procedure Assign(ASource: TIdNetPersistent); override;
    procedure BeginUpdate;
    procedure Clear; virtual; abstract;
    procedure Delete(AIndex: Integer); virtual; abstract;
    procedure EndUpdate;
    procedure Sort;
    function Equals(AObject: &Object) : Boolean; overload; override;
    function Equals(AStrings: TIdStringsFCL): Boolean; overload; 

    class operator Equal(AStrings1, AStrings2: TIdStringsFCL) : Boolean;
    class operator NotEqual(AStrings1, AStrings2: TIdStringsFCL) : Boolean;

    procedure Exchange(AIndex1, AIndex2: Integer); virtual;

    function GetText: string; virtual;
    function IndexOf(AValue: string): Integer; overload; virtual;
    function IndexOfName(AName: string): Integer; virtual;
    function IndexOfObject(AObject: &Object): Integer; virtual;
    procedure Insert(AIndex: Integer; const AValue: string); overload; virtual; abstract;
    procedure InsertObject(AIndex: Integer; AValue: string; AObject: &Object); virtual;
    procedure SetText(AText: string); virtual;

    property IsSynchronized: Boolean read GetIsSynchronized;
    property SyncRoot: &Object read GetSyncRoot;
    property Capacity: Integer read GetCapacity write SetCapacity;
    property CommaText: string read GetCommaText write SetCommaText;
    property Count: Integer read GetCount;
    property Delimiter: Char read GetDelimiter write SetDelimiter;
    property DelimitedText: string read GetDelimitedText write SetDelimitedText;
    property Names[AIndex: Integer]: string read GetName;
    property Objects[AIndex: Integer]: &Object read GetObject write PutObject;
    property QuoteChar: Char read GetQuoteChar write SetQuoteChar;
    property Values[AName: string]: string read GetValue write SetValue;
    property ValuesFromIndex[AIndex: Integer]: string read GetValueFromIndex write SetValueFromIndex;
    property NameValueSeparator: Char read GetNameValueSeparator write SetNameValueSeparator;
    property Strings[AIndex: Integer]: string read Get write Put; default;
    property Text: string read GetTextStr write SetTextStr;
  end;

  TIdStringsFCLEnumerator = class(&Object, IEnumerator)
  private
    FIndex: Integer;
    FStrings: TIdStringsFCL;
  protected
    constructor Create(AStrings: TIdStringsFCL); reintroduce;
  public
    function get_Current: &Object;
    function MoveNext: Boolean;
    procedure Reset;
  end;

  TIdStringListFCL = class(TIdStringsFCL)
  protected
    FCollection: StringCollection;
    FObjectArray: ArrayList;
    FDuplicates: TIdDuplicates;
    //
    function Get(Index: Integer): string; override;
    function GetCount: Integer; override;
    function GetObject(Index: Integer): &Object; override;
    procedure PutObject(AIndex: Integer; AObject: &Object); override;
  public
    function Add(const S: string): Integer; overload; override;
    procedure Delete(Index: Integer); override;
    procedure Insert(Index: Integer; const S: string); override;
  public
    constructor Create(AValue: StringCollection); reintroduce; overload;
    constructor Create(); overload;
    procedure CopyTo(ADest: &Array; AIndex: Integer); override;
    function AddObject(S: string; AObject: &Object): Integer; override;
    destructor Destroy; override;
    procedure Clear; override;
    class operator Implicit(const aValue: TIdStringListFCL): StringCollection;
    property Duplicates: TIdDuplicates read FDuplicates write FDuplicates;
  end;

  TIdNetListSortCompare = function (Item1, Item2: &Object): Integer;
  TIdNetListNotification = (lnAdded, lnExtracted, lnDeleted);

  TIdNetListAssignOp = (laCopy, laAnd, laOr, laXor, laSrcUnique, laDestUnique);

  EListError = class(Exception);

  TIdNetListEnumerator = class
  private
    FIndex: Integer;
    FList: TIdNetList;
  public
    constructor Create(AList: TIdNetList);
    function GetCurrent: &Object;
    function MoveNext: Boolean;
    property Current: &Object read GetCurrent;
  end;

  TIdNetList = class(TObject)
  private
    FList: System.Collections.ArrayList;
  protected
    function Get(Index: Integer): &Object;
    function GetCount: Integer;
    function GetCapacity: Integer;
    procedure Grow; virtual;
    procedure Put(Index: Integer; Item: &Object);
    procedure Notify(Instance: &Object; Action: TIdNetListNotification); virtual;
    procedure SetCapacity(NewCapacity: Integer);
    procedure SetCount(NewCount: Integer);
  public
    constructor Create;
    function Add(Item: &Object): Integer;
    procedure Clear; virtual;
    procedure Delete(Index: Integer);
                                      
    class procedure Error(const Msg: string; Data: Integer); overload; //virtual;
    procedure Exchange(Index1, Index2: Integer);
    function Expand: TIdNetList;
    function Extract(Item: &Object): &Object;
    function First: TObject;
    function GetEnumerator: TIdNetListEnumerator;
    function IndexOf(Item: &Object): Integer;
    procedure Insert(Index: Integer; Item: &Object);
    function Last: TObject;
    procedure Move(CurIndex, NewIndex: Integer);
    function Remove(Item: &Object): Integer;
    procedure Pack;
    procedure Sort(Compare: TIdNetListSortCompare);
    procedure Assign(ListA: TIdNetList; AOperator: TIdNetListAssignOp = laCopy; ListB: TIdNetList = nil);
    property Capacity: Integer read GetCapacity write SetCapacity;
    property Count: Integer read GetCount write SetCount;
    property Items[Index: Integer]: &Object read Get write Put; default;
    property List: System.Collections.ArrayList read FList;
  end;

  TIdNetCollectionItem = class;
  TIdNetCollection = class;
  TIdNetCollectionItemClass = class of TIdNetCollectionItem;
  TIdNetCollectionNotification = (cnAdded, cnExtracting, cnDeleting);

  TIdNetCollectionEnumerator = class
  private
    FIndex: Integer;
    FCollection: TIdNetCollection;
  public
    constructor Create(ACollection: TIdNetCollection);
    function GetCurrent: TIdNetCollectionItem;
    function MoveNext: Boolean;
    property Current: TIdNetCollectionItem read GetCurrent;
  end;

  TIdNetCollectionItem = class(TIdNetPersistent)
  private
    FCollection: TIdNetCollection;
    FID: Integer;
    function GetIndex: Integer;
  protected
    procedure Changed(AllItems: Boolean);
    function GetOwner: TIdNetPersistent; override;
    function GetDisplayName: string; virtual;
    procedure SetCollection(Value: TIdNetCollection); virtual;
    procedure SetIndex(Value: Integer); virtual;
    procedure SetDisplayName(const Value: string); virtual;
  public
    constructor Create(Collection: TIdNetCollection); virtual;
    destructor Destroy; override;
    procedure Assign(ASource: TIdNetPersistent); override;
    property Collection: TIdNetCollection read FCollection write SetCollection;
    property ID: Integer read FID;
    property Index: Integer read GetIndex write SetIndex;
    property DisplayName: string read GetDisplayName write SetDisplayName;
  end;

  TIdNetCollection = class(TIdNetPersistent)
  private
    FItemClass: TIdNetCollectionItemClass;
    FItems: TIdNetList;
    FUpdateCount: Integer;
    FNextID: Integer;
    function GetCount: Integer;
    procedure InsertItem(Item: TIdNetCollectionItem);
    procedure RemoveItem(Item: TIdNetCollectionItem);
  protected
    property NextID: Integer read FNextID;
    procedure Notify(Item: TIdNetCollectionItem; Action: TIdNetCollectionNotification); virtual;
    { Design-time editor support }
    procedure Changed;
    function GetItem(Index: Integer): TIdNetCollectionItem;
    procedure SetItem(Index: Integer; Value: TIdNetCollectionItem);
    procedure SetItemName(Item: TIdNetCollectionItem); virtual;
    procedure Update(Item: TIdNetCollectionItem); virtual;
    property UpdateCount: Integer read FUpdateCount;
    function GetOwner: TIdNetPersistent; override;
  public
    constructor Create(ItemClass: TIdNetCollectionItemClass);
    destructor Destroy; override;
    function Add: TIdNetCollectionItem;
    procedure BeginUpdate; virtual;
    procedure Clear;
    procedure Delete(Index: Integer);
    procedure EndUpdate; virtual;
    function FindItemID(ID: Integer): TIdNetCollectionItem;
    function GetEnumerator: TIdNetCollectionEnumerator;
    function Insert(Index: Integer): TIdNetCollectionItem;
    property Count: Integer read GetCount;
    property ItemClass: TIdNetCollectionItemClass read FItemClass;
    property Items[Index: Integer]: TIdNetCollectionItem read GetItem write SetItem;
  end;

  TIdNetThreadPriority = ThreadPriority;
  EThread = class(Exception);
  TIdNetNotifyEvent = procedure(ASender: &Object);
  TIdNetThreadMethod = procedure;
  TIdNetSynchronizeRecord = record
    FThread: &Object;
    FMethod: TIdNetThreadMethod;
    FSynchronizeException: &Object;
  end;

  TIdNetThread = class
  private
    FHandle: System.Threading.Thread;
    FCreateSuspended: Boolean;
    FStarted: Boolean;
    FSuspendCount: Integer;
    FTerminated: Boolean;
    FSuspended: Boolean;
    FFreeOnTerminate: Boolean;
    FFinished: Boolean;
    FReturnValue: Integer;
    FOnTerminate: TIdNetNotifyEvent;
    FFatalException: TObject;
    procedure ThreadError(O: &Object);
    procedure CallOnTerminate;
    function GetPriority: TIdNetThreadPriority;
    procedure SetPriority(Value: TIdNetThreadPriority);
    procedure SetSuspended(Value: Boolean);
  protected
    procedure Initialize; virtual;
    procedure DoTerminate; virtual;
    procedure Execute; virtual; abstract;
    procedure Queue(AMethod: TIdNetThreadMethod); overload;
    procedure Synchronize(Method: TIdNetThreadMethod); overload;
    property ReturnValue: Integer read FReturnValue write FReturnValue;
    property Terminated: Boolean read FTerminated;
  public
    constructor Create(CreateSuspended: Boolean);
    destructor Destroy; override; 
    procedure Resume;
    procedure Suspend;
    procedure Terminate;
    function WaitFor: LongWord; overload;
    function WaitFor(TimeOut: Integer; var ReturnValue: LongWord): Boolean; overload;
    class procedure Queue(AThread: TIdNetThread; AMethod: TIdNetThreadMethod); overload;
    class procedure RemoveQueuedEvents(AThread: TIdNetThread; AMethod: TIdNetThreadMethod);
    class procedure StaticQueue(AThread: TIdNetThread; AMethod: TIdNetThreadMethod);
    property FatalException: &Object read FFatalException;
    property FreeOnTerminate: Boolean read FFreeOnTerminate write FFreeOnTerminate;
    property Handle: System.Threading.Thread read FHandle;
    property Priority: TIdNetThreadPriority read GetPriority write SetPriority;
    property Suspended: Boolean read FSuspended write SetSuspended;
    property OnTerminate: TIdNetNotifyEvent read FOnTerminate write FOnTerminate;
  end;

  TIdNetThreadList = class
  private
    FList: TIdNetList;
    FDuplicates: TIdDuplicates;
  public
    constructor Create;
    procedure Add(Item: &Object);
    procedure Clear;
    function  LockList: TIdNetList;
    procedure Remove(Item: &Object);
    procedure UnlockList;
    property Duplicates: TIdDuplicates read FDuplicates write FDuplicates;
  end;

  TIdNetOwnedCollection = class(TIdNetCollection)
  private
    FOwner: TIdNetPersistent;
  protected
    function GetOwner: TIdNetPersistent; override;
  public
    constructor Create(AOwner: TIdNetPersistent; ItemClass: TIdNetCollectionItemClass);
  end;

const
  tpIdNetLowest = ThreadPriority.Lowest;
  tpIdNetBelowNormal = ThreadPriority.BelowNormal;
  tpIdNetNormal = ThreadPriority.Normal;
  tpIdNetAboveNormal = ThreadPriority.AboveNormal;
  tpIdNetHighest = ThreadPriority.Highest;

implementation

uses
  IdSys, IdGlobal;

const
  MaxBufSize = 5 * 1024;
  IfmCreate         = $FFFF;
  IfmOpenRead       = $0000;
  IfmOpenWrite      = $0001;
  IfmOpenReadWrite  = $0002;
  IfmShareExclusive = $0010;
  IfmShareDenyWrite = $0020;
  IfmShareDenyNone  = $0040;

resourcestring
  SReadError = 'Read Error.';
  SWriteError = 'Write Error.';
  SListCapacityError = 'List capacity out of bounds (%d)';
  SListCountError = 'List count out of bounds (%d)';
  SListIndexError = 'List index out of bounds (%d)';
  SInvalidProperty = 'Invalid property value';
  SThreadCreateError = 'Thread creation error: %s';
  SThreadError = 'Thread Error: %s (%d)';
  SCheckSynchronizeError = 'CheckSynchronize called from thread $%x, which is NOT the main thread';
  SDuplicateItem = 'List does not allow duplicates ($0%x)';

var
  SyncList: TIdNetList;
  SyncEvent: System.Threading.ManualResetEvent;
  ThreadLock: &Object;

type
  TIdNetSyncProc = record
    SyncRec: TIdNetSynchronizeRecord;
    Queued: Boolean;
    Signal: ManualResetEvent;
  end;



{ Local functions}

procedure InitThreadSynchronization;
begin
  ThreadLock := &Object.Create;
  SyncEvent := System.Threading.ManualResetEvent.Create(False);
// Should we check for errors?
//  if SyncEvent.Handle = nil then
//    ;
end;

procedure DoneThreadSynchronization;
begin
  Sys.FreeAndNil(ThreadLock);
  SyncEvent.Close;
end;

procedure ResetSyncEvent;
begin
  SyncEvent.Reset;
end;

procedure WaitForSyncEvent(Timeout: Integer);
begin
  if SyncEvent.WaitOne(Timeout, True) then
    ResetSyncEvent;
end;

procedure SignalSyncEvent;
begin
  Assert(SyncEvent<>nil);
  SyncEvent.&Set;
end;

function CheckSynchronize(Timeout: Integer = 0): Boolean;
var
  SyncProc: TIdNetSyncProc;
  LocalSyncList, TempSyncList: TIdNetList;
begin
  if System.Threading.Thread.CurrentThread <> MainThread then
    raise EThread.Create(SCheckSynchronizeError);
  if Timeout > 0 then
    WaitForSyncEvent(Timeout)
  else
    ResetSyncEvent;
  LocalSyncList := nil;
  System.Threading.Monitor.Enter(ThreadLock);
  try
    if SyncList <> nil then
    begin
      System.Threading.Monitor.Enter(SyncList);
//      Integer(LocalSyncList) := InterlockedExchange(Integer(SyncList), Integer(LocalSyncList));
      TempSyncList := SyncList;
      SyncList := LocalSyncList;
      LocalSyncList := TempSyncList;
      System.Threading.Monitor.Exit(LocalSyncList);
    end;
    try
      Result := (LocalSyncList <> nil) and (LocalSyncList.Count > 0);
      if Result then
      begin
        while LocalSyncList.Count > 0 do
        begin
          SyncProc := TIdNetSyncProc(LocalSyncList[0]);
          LocalSyncList.Delete(0);
          System.Threading.Monitor.Exit(ThreadLock);
          try
            try
              SyncProc.SyncRec.FMethod;
            except
              on E: Exception do
                SyncProc.SyncRec.FSynchronizeException := E;
            end;
          finally
            System.Threading.Monitor.Enter(ThreadLock);
          end;
          if not SyncProc.Queued then
            SyncProc.Signal.&Set;
        end;
      end;
    finally
      LocalSyncList.Free;
    end;
  finally
    System.Threading.Monitor.Exit(ThreadLock);
  end;
end;

{ TIdStringsFCL }

function TIdStringsFCL.IndexOf(AValue: string): Integer;
var
  I: Integer;
begin
  Result := -1;
  for I := 0 to Count - 1 do
  begin
    if CompareStrings(AValue, Get(I)) = 0 then
    begin
      Result := I;
      Exit;
    end;
  end;
end;

function TIdStringsFCL.IndexOf(AObject: &Object): Integer;
begin
  Result := IndexOf(string(AObject));
end;

function TIdStringsFCL.IndexOfName(AName: string): Integer;
var
  I: Integer;
  S: string;
  P: Integer;
begin
  Result := -1;
  for I := 0 to Count - 1 do
  begin
    S := Get(I);
    P := S.IndexOf(NameValueSeparator);
    if (P > 0) and (S.Substring(0, P) = AName) then
    begin
      Result := i;
      Exit;
    end;
  end;
end;

function TIdStringsFCL.AddObject(S: string; AObject: &Object): Integer;
begin
  Result := GetCount;
  Insert(Result, S);
  PutObject(Result, AObject);
end;

function TIdStringsFCL.GetIsFixedSize: Boolean;
begin
  Result := False;
end;

function TIdStringsFCL.ExtractName(S: string): string;
var
  P: Integer;
begin
  P := S.IndexOf(NameValueSeparator);
  Result := '';
  if P > 0 then
  begin
    Result := S.Substring(0, P - 1);
  end;
end;

function TIdStringsFCL.GetDelimiter: Char;
begin
  if not (sdDelimiter in FDefined) then
  begin
    Delimiter := ',';
  end;
  Result := FDelimiter;
end;

function TIdStringsFCL.Add(const S: string): Integer;
begin
  Result := AddObject(S, nil);
end;

function TIdStringsFCL.Add(AObject: &Object): Integer;
begin
  Result := Add(string(AObject));
end;

function TIdStringsFCL.GetCapacity: Integer;
begin
  Result := Count;
end;

procedure TIdStringsFCL.SetUpdateState(AUpdating: Boolean);
begin
end;

function TIdStringsFCL.AnsiQuotedStr(AValue, AQuote: string): string;
begin
  if AValue.StartsWith(AQuote) then
  begin
    if AValue.EndsWith(AQuote) then
    begin
      Result := AValue;
    end
    else
    begin
      Result := AValue + AQuote;
    end;
  end
  else
  begin
    if AValue.EndsWith(AQuote) then
    begin
      Result := AQuote + AValue;
    end
    else
    begin
      Result := AQuote + AValue + AQuote;
    end;
  end;
end;

function TIdStringsFCL.GetSyncRoot: &Object;
begin
  Result := Self;
end;

procedure TIdStringsFCL.SetTextStr(AText: string);
var
  I: Integer;
  oi: Integer;
begin
  BeginUpdate;
  try
    Clear;
    I := 0;
    while (I <> -1) and (I < AText.Length) do
    begin
      oi := I;
      I := AText.IndexOf(EOL, I+1);
      if I <> -1 then
      begin
        Add(AText.Substring(oi, I - oi));
        I := I + 2;
      end
      else
      begin
        Add(AText.Substring(oi));
        Exit;
      end;
    end;
  finally
    EndUpdate;
  end;
end;

function TIdStringsFCL.GetQuoteChar: Char;
begin
  if not (sdQuoteChar in FDefined) then
  begin
    FQuoteChar := '"';
  end;
  Result := FQuoteChar;
end;

function TIdStringsFCL.AnsiExtractQuotedStr(AValue: string; AQuote: Char): string;
var
  I: Integer;
begin
  I := AValue.IndexOf(AQuote);
  if I <> -1 then
  begin
    Result := AValue.Substring(0, I);
  end
  else
  begin
    Result := AValue;
  end;
end;

function TIdStringsFCL.GetValueFromIndex(AIndex: Integer): string;
begin
  if AIndex > -1 then
  begin
    Result := Get(AIndex).Substring(Names[AIndex].Length + 2);
  end
  else
  begin
    Result := '';
  end;
end;

procedure TIdStringsFCL.Assign(ASource: TIdNetPersistent);
var
  LSrc: TIdStringsFCL;
begin
  if ASource is TIdStringsFCL then
  begin
    LSrc := TIdStringsFCL(ASource);
    BeginUpdate;
    try
      Clear;
      FDefined := LSrc.FDefined;
      FNameValueSeparator := LSrc.FNameValueSeparator;
      FQuoteChar := LSrc.FQuoteChar;
      FDelimiter := LSrc.FDelimiter;
      AddStrings(LSrc);
    finally
      EndUpdate;
    end;
  end;
end;

function TIdStringsFCL.GetIsReadOnly: Boolean;
begin
  Result := False;
end;

function TIdStringsFCL.Equals(AStrings: TIdStringsFCL): Boolean;
var
  I: Integer;
begin
  if Count <> AStrings.Count then
  begin
    Result := False;
    Exit;
  end;

  for I := 0 to Count - 1 do
  begin
    if Get(I) <> AStrings.Get(I) then
    begin
      Result := False;
      Exit;
    end;
  end;
  Result := True;
end;

function TIdStringsFCL.Equals(AObject: &Object): Boolean;
begin
  if AObject is TIdStringsFCL then
  begin
    Result := Equals(TIdStringsFCL(AObject));
  end
  else
  begin
    Result := False;
  end;
end;

function TIdStringsFCL.GetValue(AName: string): string;
var
  I: Integer;
begin
  Result := '';
  I := IndexOfName(AName);
  if I > -1 then
  begin
    Result := Get(I).Substring(AName.Length + 1);
  end;
end;

procedure TIdStringsFCL.Exchange(AIndex1, AIndex2: Integer);
var
  DummyStr: string;
  DummyObj: &Object;
begin
  BeginUpdate;
  try
    DummyStr := Strings[AIndex1];
    DummyObj := Objects[AIndex1];
    Strings[AIndex1] := Strings[AIndex2];
    Objects[AIndex1] := Objects[AIndex2];
    Strings[AIndex2] := DummyStr;
    Objects[AIndex2] := DummyObj;
  finally
    EndUpdate;
  end;
end;

function TIdStringsFCL.CompareStrings(S1, S2: string): Integer;
begin
  Result := S1.CompareTo(S2);
end;

procedure TIdStringsFCL.SetDelimiter(AValue: Char);
begin
  if (    (FDelimiter <> AValue)
      or  (not (sdDelimiter in FDefined))
      ) then
  begin
    Include(FDefined, sdDelimiter);
    FDelimiter := AValue;
  end;
end;

function TIdStringsFCL.GetText: string;
begin
  Result := GetTextStr;
end;

function TIdStringsFCL.GetNameValueSeparator: Char;
begin
  if not (sdNameValueSeparator in FDefined) then
  begin
    NameValueSeparator := '=';
  end;
  Result := FNameValueSeparator;
end;

function TIdStringsFCL.GetIsSynchronized: Boolean;
begin
  Result := False;
end;

procedure TIdStringsFCL.SetCapacity(ACapacity: Integer);
begin
end;

procedure TIdStringsFCL.PutObject(AIndex: Integer; AObject: &Object);
begin

end;

function TIdStringsFCL.GetDelimitedText: string;
var
  LCount: Integer;
  I: Integer;
  S: string;
  P: Integer;
begin
  LCount := GetCount;
  Result := '';
  if (LCount = 0) then
  begin
    Result := QuoteChar.ToString;
  end
  else
  begin
    for I := 0 to Count - 1 do
    begin
      S := Get(I);
      P := 1;
      while (P < S.Length)
        and (S[p] > ' ')
        and (S[p] <> QuoteChar)
        and (S[p] <> Delimiter) do
      begin
        Inc(P);
      end;
      if (P < S.Length) and (S[P] <> #0) then
      begin
        S := AnsiQuotedStr(S, '' + QuoteChar);
      end;
      Result := Result + S + Delimiter;
    end;
  end;
  Result := Result.Substring(0, Result.Length - 1);
end;

function TIdStringsFCL.GetCommaText: string;
var
  OldDefined: TIdStringsDefined;
  OldDelim: Char;
  OldQuote: Char;
begin
  OldDefined := FDefined;
  OldDelim := FDelimiter;
  OldQuote := FQuoteChar;
  FDelimiter := ',';
  FQuoteChar := '"';
  try
    Result := GetDelimitedText;
  finally
    FDelimiter := OldDelim;
    FQuoteChar := OldQuote;
    FDefined := OldDefined;
  end;
end;

function TIdStringsFCL.Contains(AObject: &Object): Boolean;
begin
  Result := IndexOf(&String(AObject)) <> -1;
end;

function TIdStringsFCL.GetEnumerator: IEnumerator;
begin
  Result := TIdStringsFCLEnumerator.Create(Self);
end;

procedure TIdStringsFCL.Put(AIndex: Integer; AValue: string);
var
  Dummy: &Object;
begin
  Dummy := GetObject(AIndex);
  Delete(AIndex);
  InsertObject(AIndex, AValue, Dummy);
end;

function TIdStringsFCL.GetObject(AIndex: Integer): &Object;
begin
  Result := nil;
end;

procedure TIdStringsFCL.SetQuoteChar(AValue: Char);
begin
  if (not (sdQuoteChar in FDefined)) or (FQuoteChar <> AValue) then
  begin
    Include(FDefined, sdQuoteChar);
    FQuoteChar := AValue;
  end;
end;

class operator TIdStringsFCL.Equal(AStrings1,
  AStrings2: TIdStringsFCL): Boolean;
begin
  Result := AStrings1.Equals(AStrings2);
end;

procedure TIdStringsFCL.SetValueFromIndex(AIndex: Integer; AValue: string);
begin
  if AValue <> '' then
  begin
    if AIndex < 0 then
    begin
      AIndex := Add('');
    end;
    Put(AIndex, Names[AIndex] + NameValueSeparator + AValue);
  end
  else
  begin
    if AIndex >= 0 then
    begin
      Delete(AIndex)
    end;
  end;
end;

procedure TIdStringsFCL.EndUpdate;
begin
  Dec(FUpdateCount);
  if FUpdateCount = 0 then
  begin
    SetUpdateState(False);
  end;
end;

procedure TIdStringsFCL.Remove(AObject: &Object);
begin
  Delete(IndexOf(AObject));
end;

function TIdStringsFCL.GetName(AIndex: Integer): string;
begin
  Result := ExtractName(Get(AIndex));
end;

class operator TIdStringsFCL.NotEqual(AStrings1,
  AStrings2: TIdStringsFCL): Boolean;
begin
  Result := not AStrings1.Equals(AStrings2);
end;

procedure TIdStringsFCL.AddStrings(AStrings: TIdStringsFCL);
var
  I: Integer;
begin
  BeginUpdate;
  try
    for I := 0 to AStrings.Count - 1 do
    begin
      AddObject(AStrings[I], AStrings.Objects[I]);
    end;
  finally
    EndUpdate;
  end;
end;

procedure TIdStringsFCL.SetValue(AName, AValue: string);
var
  I: Integer;
begin
  I := IndexOfName(AName);
  if AValue <> '' then
  begin
    if I < 0 then
    begin
      I := Add('');
    end;
    Put(I, AName + NameValueSeparator + AValue);
  end
  else
  begin
    if I >= 0 then
      Delete(I);
  end;
end;

procedure TIdStringsFCL.Append(S: string);
begin
  Add(S);
end;

procedure TIdStringsFCL.SetText(AText: string);
begin
  SetTextStr(AText);
end;

procedure TIdStringsFCL.InsertObject(AIndex: Integer; AValue: string;
  AObject: &Object);
begin
  Insert(AIndex, AValue);
	PutObject(AIndex, AObject);
end;

function TIdStringsFCL.GetTextStr: string;
var
  sb: StringBuilder;
  LCount: Integer;
  I: Integer;
begin
  sb := StringBuilder.Create;
  LCount := GetCount;
  for I := 0 to LCount - 1 do
  begin
    sb.Append(Get(i) + EOL);
  end;
  Result := sb.ToString;
end;

procedure TIdStringsFCL.SetNameValueSeparator(AValue: Char);
begin
  if (FQuoteChar <> AValue) or
     (not (sdNameValueSeparator in FDefined)) then
  begin
    Include(FDefined, sdNameValueSeparator);
    FNameValueSeparator := AValue;
  end;
end;

function TIdStringsFCL.IndexOfObject(AObject: &Object): Integer;
var
  I: Integer;
begin
  Result := -1;
  for I := 0 to Count - 1 do
  begin
    if AObject = GetObject(I) then
    begin
      Result := I;
      Exit;
    end;
  end;
end;

function TIdStringsFCL.ToString: string;
begin
  Result := Text;
end;

procedure TIdStringsFCL.SetDelimitedText(AValue: string);
var
  S: string;
  P: Integer;
  P1: Integer;
begin
  BeginUpdate;
  try
    Clear;
    P := 1;
    while (P <= AValue.Length)
      and (AValue[P] > #0)
      and (AValue[P] <= #32) do
    begin
      Inc(P);
    end;
    while P <= AValue.Length do
    begin
      if AValue[P] = QuoteChar then
      begin
        S := AnsiExtractQuotedStr(AValue.Substring(P), QuoteChar);
      end
      else
      begin
        P1 := P;
        while (P <= AValue.Length)
          and (AValue[P] > #32)
          and (AValue[P] <> Delimiter) do
        begin
          Inc(P);
        end;
        S := AValue.Substring(P1 - 1, P - P1);
      end;
      Add(S);
      while (P <= AValue.Length)
        and (AValue[P] > #0)
        and (AValue[P] <= #32) do
      begin
        Inc(P);
      end;
      if (P <= AValue.Length) and
         (AValue[P] = Delimiter) then
      begin
        if (P + 1) > AValue.Length then
        begin
          Add('');
        end;
        repeat
          Inc(P);
        until not (   (P <= AValue.Length)
                  and (AValue[P] > #0)
                  and (AValue[P] <= #32));
      end;
    end;
  finally
    EndUpdate;
  end;
end;

procedure TIdStringsFCL.SetCommaText(AValue: string);
begin
  FDelimiter := ',';
  FQuoteChar := '"';
  SetDelimitedText(AValue);
end;

procedure TIdStringsFCL.BeginUpdate;
begin
  if FUpdateCount = 0 then
  begin
    SetUpdateState(True);
  end;
  Inc(FUpdateCount);
end;

procedure TIdStringsFCL.RemoveAt(AIndex: Integer);
begin
  Delete(AIndex);
end;

procedure TIdStringsFCL.Insert(AIndex: Integer; AObject: &Object);
begin
  Insert(AIndex, &String(AObject));
end;

constructor TIdStringsFCL.Create;
begin
  inherited;
  FDelimiter := ',';
  FQuoteChar := '"';
  FNameValueSeparator := '=';
  FUpdateCount := 0;
end;

class operator TIdStringsFCL.Implicit(AValue: StringCollection): TIdStringsFCL;
begin
  Result := TIdStringListFCL.Create(AValue);
end;

function TIdStringsFCL.Clone: &Object;
var
  sl: TIdStringsFCL;
begin
  sl := TIdStringListFCL.Create;
  sl.Assign(Self);
  Result := sl;
end;

class operator TIdStringsFCL.Implicit(const aValue: TIdStringsFCL): StringCollection;
begin
  EIdException.IfFalse(aValue is TIdStringListFCL, 'Invalid implicit conversion.');
  Result := TIdStringListFCL(aValue).FCollection;
end;

procedure TIdStringsFCL.SaveToStream(AStream: TIdNetStream);
begin
  raise NotImplementedException.Create;
end;

procedure TIdStringsFCL.Sort;
begin
  //
end;

procedure TIdStringsFCL.LoadFromFile(AFileName: string);
begin
  raise NotImplementedException.Create;
end;

procedure TIdStringsFCL.SaveToFile(AFileName: string);
begin
  raise NotImplementedException.Create;
end;

procedure TIdStringsFCL.LoadFromStream(AStream: TIdNetStream);
begin
  raise NotImplementedException.Create;
end;

{ TIdStringsFCLEnumerator }

constructor TIdStringsFCLEnumerator.Create(AStrings: TIdStringsFCL);
begin
  inherited Create;
  FStrings := AStrings;
  Reset;
end;

procedure TIdStringsFCLEnumerator.Reset;
begin
  FIndex := -1;
end;

function TIdStringsFCLEnumerator.get_Current: &Object;
begin
  Result := FStrings[FIndex];
end;

function TIdStringsFCLEnumerator.MoveNext: Boolean;
begin
  if FIndex < FStrings.Count then
  begin
    Inc(FIndex);
  end;
  Result := FIndex < FStrings.Count;
end;

{ TIdStringListFCL }

class operator TIdStringListFCL.Implicit(const aValue: TIdStringListFCL): StringCollection;
begin
  Result := aValue.FCollection;
end;

function TIdStringListFCL.GetCount: Integer;
begin
  Result := FCollection.Count;
end;

constructor TIdStringListFCL.Create;
begin
  Create(StringCollection.Create);
end;

constructor TIdStringListFCL.Create(AValue: StringCollection);
begin
  inherited Create;
  FObjectArray := ArrayList.Create;
  FCollection := AValue;
end;

function TIdStringListFCL.Add(const S: string): Integer;
begin
  Result := AddObject(S, nil);
end;

procedure TIdStringListFCL.PutObject(AIndex: Integer; AObject: &Object);
begin
  if (AIndex < 0) or
     (AIndex >= FCollection.Count) then
  begin
    EIdStringListErrorFCL.Toss('List index out of bounds (' + AIndex.ToString() + ')');
  end;

  //Changing;
  FObjectArray[AIndex] := AObject;
  //Changed;
end;

function TIdStringListFCL.GetObject(Index: Integer): &Object;
begin
  Result := FObjectArray.Item[Index];
end;

function TIdStringListFCL.Get(Index: Integer): string;
begin
  Result := FCollection.Item[Index];
end;

procedure TIdStringListFCL.Delete(Index: Integer);
begin
  if (Index >= 0) and (Index < Count) then
  begin
    FObjectArray.RemoveAt(Index);
    FCollection.RemoveAt(Index);
  end;
end;

procedure TIdStringListFCL.Clear;
begin
  FObjectArray.Clear;
  FCollection.Clear;
end;

destructor TIdStringListFCL.Destroy;
begin
  Clear;
  inherited;
end;

procedure TIdStringListFCL.Insert(Index: Integer; const S: string);
begin
  FCollection.Insert(Index, S);
  FObjectArray.Insert(Index, nil);
  PutObject(Index, nil);
end;

procedure TIdStringListFCL.CopyTo(ADest: &Array; AIndex: Integer);
begin
  raise NotImplementedException.Create;
end;

{ TIdNetStream }

procedure TIdNetStream.SetPosition(const Pos: Int64);
begin
  Seek(Pos, soBeginning);
end;

procedure TIdNetStream.ReadBuffer(Buffer: array of Byte; Count: Longint);
begin
  if (Count <> 0) and (Read(Buffer, Count) <> Count) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Byte);
begin
  if Read(Buffer) <> SizeOf(Byte) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Byte; Count: Longint);
begin
  if (Count <> 0) and (Read(Buffer, Count) <> Count) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Boolean);
begin
  if Read(Buffer) <> SizeOf(Boolean) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Boolean; Count: Longint);
begin
  if (Count <> 0) and (Read(Buffer, Count) <> Count) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Char);
begin
  if Read(Buffer) <> SizeOf(Char) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Char; Count: Longint);
begin
  if (Count <> 0) and (Read(Buffer, Count) <> Count) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: AnsiChar);
begin
  if Read(Buffer) <> SizeOf(AnsiChar) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: AnsiChar; Count: Longint);
begin
  if (Count <> 0) and (Read(Buffer, Count) <> Count) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: ShortInt);
begin
  if Read(Buffer) <> SizeOf(ShortInt) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: ShortInt; Count: Longint);
begin
  if (Count <> 0) and (Read(Buffer, Count) <> Count) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: SmallInt);
begin
  if Read(Buffer) <> SizeOf(SmallInt) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: SmallInt; Count: Longint);
begin
  if (Count <> 0) and (Read(Buffer, Count) <> Count) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Word);
begin
  if Read(Buffer) <> SizeOf(Word) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Word; Count: Longint);
begin
  if (Count <> 0) and (Read(Buffer, Count) <> Count) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Integer);
begin
  if Read(Buffer) <> SizeOf(Integer) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Integer; Count: Longint);
begin
  if (Count <> 0) and (Read(Buffer, Count) <> Count) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Cardinal);
begin
  if Read(Buffer) <> SizeOf(Cardinal) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Cardinal; Count: Longint);
begin
  if (Count <> 0) and (Read(Buffer, Count) <> Count) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Int64);
begin
  if Read(Buffer) <> SizeOf(Int64) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Int64; Count: Longint);
begin
  if (Count <> 0) and (Read(Buffer, Count) <> Count) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: UInt64);
begin
  if Read(Buffer) <> SizeOf(UInt64) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: UInt64; Count: Longint);
begin
  if (Count <> 0) and (Read(Buffer, Count) <> Count) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Single);
begin
  if Read(Buffer) <> SizeOf(Single) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Single; Count: Longint);
begin
  if (Count <> 0) and (Read(Buffer, Count) <> Count) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Double);
begin
  if Read(Buffer) <> SizeOf(Double) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Double; Count: Longint);
begin
  if (Count <> 0) and (Read(Buffer, Count) <> Count) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Extended);
begin
  if Read(Buffer) <> 10 then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.ReadBuffer(var Buffer: Extended; Count: Longint);
begin
  if (Count <> 0) and (Read(Buffer, Count) <> Count) then
    raise EReadError.Create(SReadError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: array of Byte; Count: Longint);
begin
  if (Count <> 0) and (Write(Buffer, Count) <> Count) then
    raise EWriteError.Create(SWriteError);
end;
{
procedure TIdNetStream.WriteBuffer(const Buffer: Byte);
begin
  if Write(Buffer) <> SizeOf(Byte) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Byte; Count: Longint);
begin
  if (Count <> 0) and (Write(Buffer, Count) <> Count) then
    raise EWriteError.Create(SWriteError);
end;
}
procedure TIdNetStream.WriteBuffer(const Buffer: Boolean);
begin
  if Write(Buffer) <> SizeOf(Boolean) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Boolean; Count: Longint);
begin
  if (Count <> 0) and (Write(Buffer, Count) <> Count) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Char);
begin
  if Write(Buffer) <> SizeOf(Char) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Char; Count: Longint);
begin
  if (Count <> 0) and (Write(Buffer, Count) <> Count) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: AnsiChar);
begin
  if Write(Buffer) <> SizeOf(AnsiChar) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: AnsiChar; Count: Longint);
begin
  if (Count <> 0) and (Write(Buffer, Count) <> Count) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: ShortInt);
begin
  if Write(Buffer) <> SizeOf(ShortInt) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: ShortInt; Count: Longint);
begin
  if (Count <> 0) and (Write(Buffer, Count) <> Count) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: SmallInt);
begin
  if Write(Buffer) <> SizeOf(SmallInt) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: SmallInt; Count: Longint);
begin
  if (Count <> 0) and (Write(Buffer, Count) <> Count) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Word);
begin
  if Write(Buffer) <> SizeOf(Word) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Word; Count: Longint);
begin
  if (Count <> 0) and (Write(Buffer, Count) <> Count) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Integer);
begin
  if Write(Buffer, 4) <> 4 then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Integer; Count: Longint);
begin
  if (Count <> 0) and (Write(Buffer, Count) <> Count) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Cardinal);
begin
  if Write(Buffer) <> SizeOf(Cardinal) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Cardinal; Count: Longint);
begin
  if (Count <> 0) and (Write(Buffer, Count) <> Count) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Int64);
begin
  if Write(Buffer) <> SizeOf(Int64) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Int64; Count: Integer);
begin
  if (Count <> 0) and (Write(Buffer, Count) <> Count) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: UInt64);
begin
  if Write(Buffer) <> SizeOf(UInt64) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: UInt64; Count: Integer);
begin
  if (Count <> 0) and (Write(Buffer, Count) <> Count) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Single);
begin
  if Write(Buffer) <> SizeOf(Single) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Single; Count: Integer);
begin
  if (Count <> 0) and (Write(Buffer, Count) <> Count) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Double);
begin
  if Write(Buffer) <> SizeOf(Double) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Double; Count: Integer);
begin
  if (Count <> 0) and (Write(Buffer, Count) <> Count) then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Extended);
begin
  if Write(Buffer) <> 10 then
    raise EWriteError.Create(SWriteError);
end;

procedure TIdNetStream.WriteBuffer(const Buffer: Extended; Count: Integer);
begin
  if (Count <> 0) and (Write(Buffer, Count) <> Count) then
    raise EWriteError.Create(SWriteError);
end;

function TIdNetStream.CopyFrom(Source: TIdNetStream; Count: Int64): Int64;
var
  BufSize, N: Integer;
  Buffer: array of Byte;
begin
  if Count = 0 then
  begin
    Source.Position := 0;
    Count := Source.Size;
  end;
  Result := Count;
  if Count > MaxBufSize then
    BufSize := MaxBufSize
  else
    BufSize := Count;
  SetLength(Buffer, BufSize);
  while Count <> 0 do
  begin
    if Count > BufSize then
      N := BufSize
    else
      N := Count;
    Source.ReadBuffer(Buffer, N);
    WriteBuffer(Buffer, N);
    Dec(Count, N);
  end;
end;

class operator TIdNetStream.Implicit(const Value: TIdNetStream): System.IO.Stream;
begin
  if Value is TIdNetCLRStreamWrapper then
    Result := TIdNetCLRStreamWrapper(Value).Handle
  else
    Result := TIdNetWrapperFCLStream.Create(Value);
end;

class operator TIdNetStream.Implicit(const Value: System.IO.Stream): TIdNetStream;
begin
  Result := TIdNetCLRStreamWrapper.Create(Value);
end;

function TIdNetStream.Read(var Buffer: array of Byte; Count: Longint): Longint;
begin
  Result := Read(Buffer, 0, Count);
end;

function TIdNetStream.Read(var Buffer: Byte): Longint;
var
  Buf: array[] of System.Byte;
begin
  Result := Read(Buf, 0, 1);
  Buffer := Buf[0];
end;

function TIdNetStream.Read(var Buffer: Byte; Count: Longint): Longint;
var
  Buf: array[] of Byte;
begin
  if Count <> 0 then
  begin
    Result := Read(Buf, 1);
    if Count > 1 then
      Inc(Result, Skip(Count - 1));
    Buffer := Buf[0];
  end
  else
    Result := 0;
end;

function TIdNetStream.Read(var Buffer: Boolean): Longint;
var
  Buf: array of Byte;
begin
  Result := Read(Buf, 1);
  Buffer := Boolean(Buf[0]);
end;

function TIdNetStream.Read(var Buffer: Boolean; Count: Longint): Longint;
var
  Buf: array[] of Byte;
begin
  if Count <> 0 then
  begin
    Result := Read(Buf, 1);
    if Count > 1 then
      Inc(Result, Skip(Count - 1));
    Buffer := Boolean(Buf[0]);
  end
  else
    Result := 0;
end;

function TIdNetStream.Read(var Buffer: Char): Longint;
var
  Buf: array[] of Byte;
begin
  Result := Read(Buf, 2);
  Buffer := Char(Buf[0] or Buf[1] shl 8);
end;

function TIdNetStream.Read(var Buffer: Char; Count: Longint): Longint;
var
  Buf: array[] of Byte;
  S: Integer;
begin
  S := 0;
  if Count > 2 then
  begin
    S := Count - 2;
    Count := 2;
  end;
  if Count <> 0 then
  begin
    Buf[1] := 0;
    Result := Read(Buf, Count);
    Buffer := Char(Buf[0] or Buf[1] shl 8);
    if S <> 0 then
      Inc(Result, Skip(S));
  end
  else
    Result := 0;
end;

function TIdNetStream.Read(var Buffer: AnsiChar): Longint;
var
  Buf: array[] of Byte;
begin
  Result := Read(Buf, 1);
  Buffer := AnsiChar(Buf[0]);
end;

function TIdNetStream.Read(var Buffer: AnsiChar; Count: Longint): Longint;
var
  Buf: array[] of Byte;
begin
  if Count <> 0 then
  begin
    Result := Read(Buf, 1);
    if Count > 1 then
      Inc(Result, Skip(Count - 1));
    Buffer := AnsiChar(Buf[0]);
  end
  else
    Result := 0;
end;

function TIdNetStream.Read(var Buffer: ShortInt): Longint;
var
  Buf: array[] of Byte;
begin
  Result := Read(Buf, 1);
  Buffer := ShortInt(Buf[0]);
end;

function TIdNetStream.Read(var Buffer: ShortInt; Count: Longint): Longint;
var
  Buf: array[] of Byte;
begin
  if Count <> 0 then
  begin
    Result := Read(Buf, 1);
    if Count > 1 then
      Inc(Result, Skip(Count - 1));
    Buffer := ShortInt(Buf[0]);
  end
  else
    Result := 0;
end;

function TIdNetStream.Read(var Buffer: SmallInt): Longint;
var
  Buf: array[] of Byte;
begin
  Result := Read(Buf, 2);
  Buffer := SmallInt(Buf[0] or (Buf[1] shl 8));
end;

function TIdNetStream.Read(var Buffer: SmallInt; Count: Longint): Longint;
var
  Buf: array[] of Byte;
  S: Integer;
begin
  S := 0;
  if Count > 2 then
  begin
    S := Count - 2;
    Count := 2;
  end;
  if Count <> 0 then
  begin
    Buf[1] := 0;
    Result := Read(Buf, Count);
    Buffer := SmallInt(Buf[0] or (Buf[1] shl 8));
    if S <> 0 then
      Inc(Result, Skip(S));
  end
  else
    Result := 0;
end;

function TIdNetStream.Read(var Buffer: Word): Longint;
var
  Buf: array[] of Byte;
begin
  Result := Read(Buf, 2);
  Buffer := Word(Buf[0] or (Buf[1] shl 8));
end;

function TIdNetStream.Read(var Buffer: Word; Count: Longint): Longint;
var
  Buf: array[] of Byte;
  S: Integer;
begin
  S := 0;
  if Count > 2 then
  begin
    S := Count - 2;
    Count := 2;
  end;
  if Count <> 0 then
  begin
    Buf[1] := 0;
    Result := Read(Buf, Count);
    Buffer := Word(Buf[0] or (Buf[1] shl 8));
    if S <> 0 then
      Inc(Result, Skip(S));
  end
  else
    Result := 0;
end;

function TIdNetStream.Read(var Buffer: Integer): Longint;
var
  Buf: array[] of Byte;
begin
  Result := Read(Buf, 4);
  Buffer := Integer(Buf[0] or (Buf[1] shl 8) or (Buf[2] shl 16) or (Buf[3] shl 24));
end;

function TIdNetStream.Read(var Buffer: Integer; Count: Longint): Longint;
var
  Buf: array[] of Byte;
  S: Integer;
begin
  S := 0;
  if Count > 4 then
  begin
    S := Count - 4;
    Count := 4;
  end;
  if Count <> 0 then
  begin
    Buf[1] := 0;
    Buf[2] := 0;
    Buf[3] := 0;
    Result := Read(Buf, Count);
    Buffer := Integer(Buf[0] or (Buf[1] shl 8) or (Buf[2] shl 16) or (Buf[3] shl 24));
    if S <> 0 then
      Inc(Result, Skip(S));
  end
  else
    Result := 0;
end;

function TIdNetStream.Read(var Buffer: Cardinal): Longint;
var
  Buf: array[] of Byte;
begin
  Result := Read(Buf, 4);
  Buffer := Cardinal(Buf[0] or (Buf[1] shl 8) or (Buf[2] shl 16) or (Buf[3] shl 24));
end;

function TIdNetStream.Read(var Buffer: Cardinal; Count: Longint): Longint;
var
  Buf: array[] of Byte;
  S: Integer;
begin
  S := 0;
  if Count > 4 then
  begin
    S := Count - 4;
    Count := 4;
  end;
  if Count <> 0 then
  begin
    Buf[1] := 0;
    Buf[2] := 0;
    Buf[3] := 0;
    Result := Read(Buf, Count);
    Buffer := Cardinal(Buf[0] or (Buf[1] shl 8) or (Buf[2] shl 16) or (Buf[3] shl 24));
    if S <> 0 then
      Inc(Result, Skip(S));
  end
  else
    Result := 0;
end;

function TIdNetStream.Read(var Buffer: Int64): Longint;
var
  Buf: array[] of Byte;
begin
  Result := Read(Buf, 8);
  Buffer := Int64(Buf[0]) or (Int64(Buf[1]) shl 8) or
      (Int64(Buf[2]) shl 16) or (Int64(Buf[3]) shl 24) or
      (Int64(Buf[4]) shl 32) or (Int64(Buf[5]) shl 40) or
      (Int64(Buf[6]) shl 48) or (Int64(Buf[7]) shl 56);
end;

function TIdNetStream.Read(var Buffer: Int64; Count: Longint): Longint;
var
  Buf: array[] of Byte;
  S: Integer;
begin
  S := 0;
  if Count > 8 then
  begin
    S := Count - 8;
    Count := 8;
  end;
  if Count <> 0 then
  begin
    Buf[1] := 0;
    Buf[2] := 0;
    Buf[3] := 0;
    Buf[4] := 0;
    Buf[5] := 0;
    Buf[6] := 0;
    Buf[7] := 0;
    Result := Read(Buf, Count);
    Buffer := Int64(Buf[0]) or (Int64(Buf[1]) shl 8) or
        (Int64(Buf[2]) shl 16) or (Int64(Buf[3]) shl 24) or
        (Int64(Buf[4]) shl 32) or (Int64(Buf[5]) shl 40) or
        (Int64(Buf[6]) shl 48) or (Int64(Buf[7]) shl 56);
    if S <> 0 then
      Inc(Result, Skip(S));
  end
  else
    Result := 0;
end;

function TIdNetStream.Read(var Buffer: UInt64): Longint;
var
  Buf: array[] of Byte;
begin
  Result := Read(Buf, 8);
  Buffer := Int64(Buf[0]) or (Int64(Buf[1]) shl 8) or
      (Int64(Buf[2]) shl 16) or (Int64(Buf[3]) shl 24) or
      (Int64(Buf[4]) shl 32) or (Int64(Buf[5]) shl 40) or
      (Int64(Buf[6]) shl 48) or (Int64(Buf[7]) shl 56);
end;

function TIdNetStream.Read(var Buffer: UInt64; Count: Longint): Longint;
var
  Buf: array[] of Byte;
  S: Integer;
begin
  S := 0;
  if Count > 8 then
  begin
    S := Count - 8;
    Count := 8;
  end;
  if Count <> 0 then
  begin
    Buf[1] := 0;
    Buf[2] := 0;
    Buf[3] := 0;
    Buf[4] := 0;
    Buf[5] := 0;
    Buf[6] := 0;
    Buf[7] := 0;
    Result := Read(Buf, Count);
    Buffer := Int64(Buf[0]) or (Int64(Buf[1]) shl 8) or
        (Int64(Buf[2]) shl 16) or (Int64(Buf[3]) shl 24) or
        (Int64(Buf[4]) shl 32) or (Int64(Buf[5]) shl 40) or
        (Int64(Buf[6]) shl 48) or (Int64(Buf[7]) shl 56);
    if S <> 0 then
      Inc(Result, Skip(S));
  end
  else
    Result := 0;
end;

function TIdNetStream.Read(var Buffer: Single): Longint;
var
  Buf: array[] of Byte;
begin
  Result := Read(Buf, 4);
  Buffer := BitConverter.ToSingle(Buf, 0);
end;

function TIdNetStream.Read(var Buffer: Single; Count: Longint): Longint;
var
  Buf: array[] of Byte;
begin
  if Count <> 4 then
  begin
    Buffer := 0;
    Result := Skip(Count);
  end
  else
  begin
    Result := Read(Buf, 4);
    Buffer := BitConverter.ToSingle(Buf, 0);
  end;
end;

function TIdNetStream.Read(var Buffer: Double): Longint;
var
  Buf: array[] of Byte;
begin
  Result := Read(Buf, 8);
  Buffer := BitConverter.ToDouble(Buf, 0);
end;

function TIdNetStream.Read(var Buffer: Double; Count: Longint): Longint;
var
  Buf: array[] of Byte;
begin
  if Count = 8 then
  begin
    Result := Read(Buf, 8);
    Buffer := BitConverter.ToDouble(Buf, 0);
  end
  else
  begin
    Buffer := 0;
    Result := Skip(Count);
  end;
end;

function TIdNetStream.Read(var Buffer: Extended): Longint;
var
  Buf: array[] of Byte;
begin
  // Read Win32 compatible extended
  Result := Read(Buf, 10);
  Buffer := ExtendedAsBytesToDouble(Buf);
end;

function TIdNetStream.Read(var Buffer: Extended; Count: Longint): Longint;
var
  Buf: array[] of Byte;
begin
  if Count = SizeOf(Double) then
  begin
    Result := Read(Buf, SizeOf(Double));
    Buffer := BitConverter.ToDouble(Buf, 0);
  end
  else if Count = 10 then
  begin
    // Read Win32 compatible extended
    Result := Read(Buf, 10);
    Buffer := ExtendedAsBytesToDouble(Buf);
  end
  else
  begin
    Buffer := 0;
    Result := Skip(Count);
  end;
end;

function TIdNetStream.Skip(Amount: Integer): Integer;
var
  P: Integer;
begin
  P := Position;
  Result := Seek(Amount, soCurrent) - P;
end;

function TIdNetStream.Write(const Buffer: array of Byte; Count: Longint): Longint;
begin
  Result := Write(Buffer, 0, Count);
end;
{
function TIdNetStream.Write(const Buffer: Byte): Longint;
var
  Buf: array[] of Byte;
begin
  Buf[0] := Buffer;
  Result := Write(Buf, 1);
end;

function TIdNetStream.Write(const Buffer: Byte; Count: Longint): Longint;
var
  Buf: array[] of Byte;
  C: Integer;
begin
  C := Count;
  if C > 1 then
    C := 1;
  Buf[0] := Buffer;
  Result := Write(Buf, C);
  if C < Count then
    Inc(Result, Skip(Count - C));
end;
}
function TIdNetStream.Write(const Buffer: Boolean): Longint;
var
  Buf: array[] of Byte;
begin
  Buf[0] := Byte(Buffer);
  Result := Write(Buf, 1);
end;

function TIdNetStream.Write(const Buffer: Boolean; Count: Longint): Longint;
var
  Buf: array[] of Byte;
  C: Integer;
begin
  C := Count;
  if C > 1 then
    C := 1;
  Buf[0] := Byte(Buffer);
  Result := Write(Buf, C);
  if C < Count then
    Inc(Result, Skip(Count - C));
end;

function TIdNetStream.Write(const Buffer: Char): Longint;
var
  Buf: array[] of Byte;
begin
  Buf[0] := Word(Buffer) and $FF;
  Buf[1] := (Word(Buffer) shr 8) and $FF;
  Result := Write(Buf, 2);
end;

function TIdNetStream.Write(const Buffer: Char; Count: Longint): Longint;
var
  Buf: array[] of Byte;
  C: Integer;
begin
  C := Count;
  if C > 2 then
    C := 2;
  Buf[0] := Word(Buffer) and $FF;
  Buf[1] := (Word(Buffer) shr 8) and $FF;
  Result := Write(Buf, C);
  if C < Count then
    Inc(Result, Skip(Count - C));
end;

function TIdNetStream.Write(const Buffer: AnsiChar): Longint;
var
  Buf: array[] of Byte;
begin
  Buf[0] := Byte(Buffer);
  Result := Write(Buf, 1);
end;

function TIdNetStream.Write(const Buffer: AnsiChar; Count: Longint): Longint;
var
  Buf: array[] of Byte;
  C: Integer;
begin
  C := Count;
  if C > 1 then
    C := 1;
  Buf[0] := Byte(Buffer);
  Result := Write(Buf, C);
  if C < Count then
    Inc(Result, Skip(Count - C));
end;

function TIdNetStream.Write(const Buffer: ShortInt): Longint;
var
  Buf: array[] of Byte;
begin
  Buf[0] := Buffer;
  Result := Write(Buf, 1);
end;

function TIdNetStream.Write(const Buffer: ShortInt; Count: Longint): Longint;
var
  Buf: array[] of Byte;
  C: Integer;
begin
  C := Count;
  if C > 1 then
    C := 1;
  Buf[0] := Buffer;
  Result := Write(Buf, C);
  if C < Count then
    Inc(Result, Skip(Count - C));
end;

function TIdNetStream.Write(const Buffer: SmallInt): Longint;
var
  Buf: array[] of Byte;
begin
  Buf[0] := Buffer and $FF;
  Buf[1] := (Buffer shr 8) and $FF;
  Result := Write(Buf, 2);
end;

function TIdNetStream.Write(const Buffer: SmallInt; Count: Longint): Longint;
var
  Buf: array[] of Byte;
  C: Integer;
begin
  C := Count;
  if C > 2 then
    C := 2;
  Buf[0] := Buffer and $FF;
  Buf[1] := (Buffer shr 8) and $FF;
  Result := Write(Buf, C);
  if C < Count then
    Inc(Result, Skip(Count - C));
end;

function TIdNetStream.Write(const Buffer: Word): Longint;
var
  Buf: array[] of Byte;
begin
  Buf[0] := Word(Buffer) and $FF;
  Buf[1] := (Word(Buffer) shr 8) and $FF;
  Result := Write(Buf, 2);
end;

function TIdNetStream.Write(const Buffer: Word; Count: Longint): Longint;
var
  Buf: array[] of Byte;
  C: Integer;
begin
  C := Count;
  if C > 2 then
    C := 2;
  Buf[0] := Word(Buffer) and $FF;
  Buf[1] := (Word(Buffer) shr 8) and $FF;
  Result := Write(Buf, C);
  if C < Count then
    Inc(Result, Skip(Count - C));
end;

function TIdNetStream.Write(const Buffer: Integer): Longint;
var
  Buf: array[] of Byte;
begin
  Buf[0] := Buffer and $FF;
  Buf[1] := (Buffer shr 8) and $FF;
  Buf[2] := (Buffer shr 16) and $FF;
  Buf[3] := (Buffer shr 24) and $FF;
  Result := Write(Buf, 4);
end;

function TIdNetStream.Write(const Buffer: Integer; Count: Longint): Longint;
var
  Buf: array[] of Byte;
  C: Integer;
begin
  C := Count;
  if C > 4 then
    C := 4;
  Buf[0] := Buffer and $FF;
  Buf[1] := (Buffer shr 8) and $FF;
  Buf[2] := (Buffer shr 16) and $FF;
  Buf[3] := (Buffer shr 24) and $FF;
  Result := Write(Buf, C);
  if C < Count then
    Inc(Result, Skip(Count - C));
end;

function TIdNetStream.Write(const Buffer: Cardinal): Longint;
var
  Buf: array[] of Byte;
begin
  Buf[0] := Buffer and $FF;
  Buf[1] := (Buffer shr 8) and $FF;
  Buf[2] := (Buffer shr 16) and $FF;
  Buf[3] := (Buffer shr 24) and $FF;
  Result := Write(Buf, 4);
end;

function TIdNetStream.Write(const Buffer: Cardinal; Count: Longint): Longint;
var
  Buf: array[] of Byte;
  C: Integer;
begin
  C := Count;
  if C > 4 then
    C := 4;
  Buf[0] := Buffer and $FF;
  Buf[1] := (Buffer shr 8) and $FF;
  Buf[2] := (Buffer shr 16) and $FF;
  Buf[3] := (Buffer shr 24) and $FF;
  Result := Write(Buf, C);
  if C < Count then
    Inc(Result, Skip(Count - C));
end;

function TIdNetStream.Write(const Buffer: Int64): Longint;
begin
  Result := Write(BitConverter.GetBytes(Buffer), SizeOf(Int64));
end;

function TIdNetStream.Write(const Buffer: Int64; Count: Integer): Longint;
var
  C: Integer;
begin
  C := Count;
  if C > 8 then
    C := 8;
  Result := Write(BitConverter.GetBytes(Buffer), C);
  if C < Count then
    Inc(Result, Skip(Count - C));
end;

function TIdNetStream.Write(const Buffer: UInt64): Longint;
begin
  Result := Write(BitConverter.GetBytes(Buffer), SizeOf(UInt64));
end;

function TIdNetStream.Write(const Buffer: UInt64; Count: Integer): Longint;
var
  C: Integer;
begin
  C := Count;
  if C > 8 then
    C := 8;
  Result := Write(BitConverter.GetBytes(Buffer), C);
  if C < Count then
    Inc(Result, Skip(Count - C));
end;

function TIdNetStream.Write(const Buffer: Single): Longint;
begin
  Result := Write(BitConverter.GetBytes(Buffer), SizeOf(Single));
end;

function TIdNetStream.Write(const Buffer: Single; Count: Integer): Longint;
var
  C: Integer;
begin
  C := Count;
  if C > 4 then
    C := 4;
  Result := Write(BitConverter.GetBytes(Buffer), C);
  if C < Count then
    Inc(Result, Skip(Count - C));
end;

function TIdNetStream.Write(const Buffer: Double): Longint;
begin
  Result := Write(BitConverter.GetBytes(Buffer), SizeOf(Double));
end;

function TIdNetStream.Write(const Buffer: Double; Count: Integer): Longint;
var
  C: Integer;
begin
  C := Count;
  if C > 8 then
    C := 8;
  Result := Write(BitConverter.GetBytes(Buffer), C);
  if C < Count then
    Inc(Result, Skip(Count - C));
end;

function TIdNetStream.Write(const Buffer: Extended): Longint;
begin
  // Write Win32 compatible extended
  Result := Write(DoubleToExtendedAsBytes(Buffer), 10);
end;

function TIdNetStream.Write(const Buffer: Extended; Count: Longint): Longint;
begin
  if Count = SizeOf(Double) then
  begin
    Result := Write(BitConverter.GetBytes(Double(Buffer)), SizeOf(Double));
  end
  else if Count = 10 then
    // Write Win32 compatible extended
    Result := Write(DoubleToExtendedAsBytes(Buffer), 10)
  else
    Result := Skip(Count);
end;

{ TIdNetCLRStreamWrapper }

constructor TIdNetCLRStreamWrapper.Create(AHandle: System.IO.Stream);
begin
  inherited Create;
  FHandle := AHandle;
end;

destructor TIdNetCLRStreamWrapper.Destroy;
begin
  if FHandle <> nil then
    FHandle.Close;
  inherited Destroy;
end;

function TIdNetCLRStreamWrapper.GetSize: Int64;
begin
  Result := FHandle.Length;
end;

function TIdNetCLRStreamWrapper.GetPosition: Int64;
begin
  Result := FHandle.Position;
end;

function TIdNetCLRStreamWrapper.Read(var Buffer: array of Byte; Offset, Count: Longint): Longint;
begin
  Result := FHandle.Read(Buffer, Offset, Count);
end;

const
  OriginMap: array[TIdNetSeekOrigin] of System.IO.SeekOrigin =
    (System.IO.SeekOrigin.Begin, System.IO.SeekOrigin.Current,
    System.IO.SeekOrigin.End);

function TIdNetCLRStreamWrapper.Seek(const Offset: Int64; Origin: TIdNetSeekOrigin): Int64;
begin
  Result := FHandle.Seek(Offset, OriginMap[Origin]);
end;

procedure TIdNetCLRStreamWrapper.SetSize(AValue: Int64);
begin
  FHandle.SetLength(AValue);
end;

function TIdNetCLRStreamWrapper.Write(const Buffer: array of Byte; Offset, Count: Longint): Longint;
begin
  try
    FHandle.Write(Buffer, Offset, Count);
    Result := Count;
  except
    Result := 0;
  end;
end;

{ TIdNetWrapperFCLStream }

constructor TIdNetWrapperFCLStream.Create(Stream: TIdNetStream);
begin
  inherited Create;
  FStream := Stream;
end;

procedure TIdNetWrapperFCLStream.Close;
begin
  FStream.Free;
  FStream := nil;
end;

procedure TIdNetWrapperFCLStream.Flush;
begin
  // Nothing applicable
end;

function TIdNetWrapperFCLStream.get_CanRead: Boolean;
begin
  Result := True;
end;

function TIdNetWrapperFCLStream.get_CanSeek: Boolean;
begin
  Result := True;
end;

function TIdNetWrapperFCLStream.get_CanWrite: Boolean;
begin
  Result := True;
end;

function TIdNetWrapperFCLStream.get_Length: Int64;
begin
  Result := FStream.Length;
end;

function TIdNetWrapperFCLStream.get_Position: Int64;
begin
  Result := FStream.Position;
end;

function TIdNetWrapperFCLStream.Read(Buffer: array of Byte; Offset: Integer; Count: Integer): Integer;
begin
  Result := FStream.Read(Buffer, Offset, Count);
end;

function TIdNetWrapperFCLStream.Seek(Offset: Int64; Origin: System.IO.SeekOrigin): Int64;
var
  LOrigin: SeekOrigin;
begin
  case Origin of
    SeekOrigin.Current:
      LOrigin := SeekOrigin.Current;
    SeekOrigin.End:
      LOrigin := SeekOrigin.End;
  else
    LOrigin := SeekOrigin.Begin;
  end;
  Result := FStream.Seek(Offset, LOrigin);
end;

procedure TIdNetWrapperFCLStream.SetLength(Value: Int64);
begin
  FStream.SetLength(Value);
end;

procedure TIdNetWrapperFCLStream.set_Position(Value: Int64);
begin
  FStream.Position := Value;
end;

procedure TIdNetWrapperFCLStream.Write(Buffer: array of Byte; Offset: Integer; Count: Integer);
begin
  FStream.Write(Buffer, Offset, Count);
end;

destructor TIdNetWrapperFCLStream.Destroy;
begin
  FStream.Free;
  inherited;
end;

class function TIdNetWrapperFCLStream.GetStream(Stream: TIdNetStream): System.IO.Stream;
begin
  if Stream is TIdNetCLRStreamWrapper then
    Result := TIdNetCLRStreamWrapper(Stream).Handle
  else
    Result := TIdNetWrapperFCLStream.Create(Stream);
end;

{ TIdNetMemoryStream }

constructor TIdNetMemoryStream.Create;
begin
  inherited;
  FFCLStream := MemoryStream.Create;
end;

destructor TIdNetMemoryStream.Destroy;
begin
  FFCLStream.Close;
  FFCLStream := nil;
  inherited;
end;

function TIdNetMemoryStream.GetSize: Int64;
begin
  Result := FFCLStream.Length;
end;

function TIdNetMemoryStream.GetPosition: Int64;
begin
  Result := FFCLStream.Position;
end;

procedure TIdNetMemoryStream.SetSize(AValue: Int64);
begin
  FFCLStream.SetLength(AValue);
end;

function TIdNetMemoryStream.Write(const Buffer: array of Byte; Offset,
  Count: Integer): Integer;
begin
  Result := Count;
  FFCLStream.Write(Buffer, Offset, Count);
end;

function TIdNetMemoryStream.Seek(const Offset: Int64;
  Origin: TIdNetSeekOrigin): Int64;
  function GetSeekOrigin: SeekOrigin;
  begin
    Result := SeekOrigin.&Begin;
    case Origin of
      soBeginning:  Result := SeekOrigin.&Begin;
      soCurrent:    Result := SeekOrigin.Current;
      soEnd:        Result := SeekOrigin.&End;
    end;
  end;
begin
  Result := FFCLStream.Seek(Offset, GetSeekOrigin);
end;

function TIdNetMemoryStream.Read(var Buffer: array of Byte; Offset,
  Count: Integer): Integer;
begin
  Result := FFCLStream.Read(Buffer, Offset, Count);
end;

function TIdNetMemoryStream.GetMemory: TByteArray;
begin
  Result := FFCLStream.GetBuffer;
end;

{ TIdNetStringStream }

constructor TIdNetStringStream.Create(const AString: string);
begin
  inherited Create;
  WriteString(AString);
  Position := 0; 
end;

function TIdNetStringStream.GetString: string;
begin
  Result := System.Text.Encoding.ASCII.GetString(FFCLStream.GetBuffer, 0, Size)
end;

procedure TIdNetStringStream.WriteString(const AString: string);
var
  Bytes: TBytes;
begin
  Bytes := System.Text.Encoding.ASCII.GetBytes(AString);
  Write(Bytes, Length(Bytes));
end;


{ TIdNetFileStream }

function TIdNetFileStream.Write(const Buffer: array of Byte; Offset,
  Count: Integer): Longint;
begin
  Result := Count;
  FFCLStream.Write(Buffer, Offset, Count);
end;

constructor TIdNetFileStream.Create(const AFileName: string; const AMode: UInt16; const ARight: Cardinal);
var
  LMode: System.IO.FileMode;
  LAccess: System.IO.FileAccess;
  LShare: System.IO.FileShare;
begin
  inherited Create;
  if AMode = IfmCreate then
  begin
    LMode := System.IO.FileMode.Create;
    LAccess := System.IO.FileAccess.ReadWrite;
  end
  else
  begin
    LMode := System.IO.FileMode.Open;
    case AMode and $F of
      IfmOpenReadWrite: LAccess := System.IO.FileAccess.ReadWrite;
      IfmOpenWrite: LAccess := System.IO.FileAccess.Write;
    else
      LAccess := System.IO.FileAccess.Read;
    end;
  end;
  case AMode and $F0 of
    IfmShareDenyWrite: LShare := System.IO.FileShare.Read;
    IfmShareDenyNone: LShare := System.IO.FileShare.None;
  else
    LShare := System.IO.FileShare.ReadWrite;
  end;
  FFCLStream := System.IO.FileStream.Create(AFileName, LMode, LAccess, LShare);
end;

function TIdNetFileStream.GetSize: Int64;
begin
  Result := FFCLStream.Length;
end;

function TIdNetFileStream.Seek(const Offset: Int64;
  Origin: TIdNetSeekOrigin): Int64;
function GetSeekOrigin: SeekOrigin;
  begin
    Result := SeekOrigin.&Begin;
    case Origin of
      soBeginning:  Result := SeekOrigin.&Begin;
      soCurrent:    Result := SeekOrigin.Current;
      soEnd:        Result := SeekOrigin.&End;
    end;                                                                    
  end;
begin
  Result := FFCLStream.Seek(Offset, GetSeekOrigin);
end;

function TIdNetFileStream.GetPosition: Int64;
begin
  Result := FFCLStream.Position;
end;

procedure TIdNetFileStream.SetSize(AValue: Int64);
begin
  FFCLStream.SetLength(AValue);
end;

function TIdNetFileStream.Read(var Buffer: array of Byte; Offset,
  Count: Integer): Longint;
begin
  Result := FFCLStream.Read(Buffer, Offset, Count);
end;

destructor TIdNetFileStream.Destroy;
begin
  FFCLStream.Flush;
  FFCLStream.Close;
  FFCLStream := nil;
  inherited;
end;

constructor TIdNetFileStream.Create(const AFileName: string; const AMode: UInt16);
begin
  Create(AFileName, AMode, 0);
end;

type
  TIdNetListComparer = class(&Object, IComparer)
  private
    FCompare: TIdNetListSortCompare;
  public
    function Compare(O1, O2: &Object): Integer;
    constructor Create(Compare: TIdNetListSortCompare);
  end;

{ TListComparer }

function TIdNetListComparer.Compare(O1, O2: &Object): Integer;
begin
  Result := FCompare(O1, O2);
end;

constructor TIdNetListComparer.Create(Compare: TIdNetListSortCompare);
begin
  inherited Create;
  FCompare := Compare;
end;

{ TIdNetList }

constructor TIdNetList.Create;
begin
  inherited Create;
  FList := System.Collections.ArrayList.Create;
end;

function TIdNetList.Add(Item: &Object): Integer;
begin
  Result := FList.Add(Item);
  if Item <> nil then
    Notify(Item, lnAdded);
end;

procedure TIdNetList.Clear;
begin
  FList.Clear;
end;

procedure TIdNetList.Delete(Index: Integer);
var
  Temp: &Object;
begin
  Temp := FList[Index];
  FList.RemoveAt(Index);
  if Temp <> nil then
    Notify(Temp, lnDeleted);
end;

class procedure TIdNetList.Error(const Msg: string; Data: Integer);
begin
  raise EListError.Create(Sys.Format(Msg, [Data]));
end;

procedure TIdNetList.Exchange(Index1, Index2: Integer);
var
  Item: &Object;
begin
  Item := FList[Index1];
  FList[Index1] := FList[Index2];
  FList[Index2] := Item;
end;

function TIdNetList.Expand: TIdNetList;
begin
  if FList.Count = FList.Capacity then
    Grow;
  Result := Self;
end;

function TIdNetList.First: &Object;
begin
  Result := Get(0);
end;

function TIdNetList.Get(Index: Integer): &Object;
begin
  Result := FList[Index];
end;

function TIdNetList.GetCapacity: Integer;
begin
  Result := FList.Capacity;
end;

function TIdNetList.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TIdNetList.GetEnumerator: TIdNetListEnumerator;
begin
  Result := TIdNetListEnumerator.Create(Self);
end;

procedure TIdNetList.Grow;
var
  Delta: Integer;
  LCapacity: Integer;
begin
  LCapacity := FList.Capacity;
  if LCapacity > 64 then
    Delta := LCapacity div 4
  else
    if LCapacity > 8 then
      Delta := 16
    else
      Delta := 4;
  SetCapacity(LCapacity + Delta);
end;

function TIdNetList.IndexOf(Item: &Object): Integer;
begin
  Result := FList.IndexOf(&Object(Item));
end;

procedure TIdNetList.Insert(Index: Integer; Item: &Object);
begin
  FList.Insert(Index, Item);
  if Item <> nil then
    Notify(Item, lnAdded);
end;

function TIdNetList.Last: &Object;
begin
  Result := Get(Count - 1);
end;

procedure TIdNetList.Move(CurIndex, NewIndex: Integer);
var
  Item: &Object;
begin
  if CurIndex <> NewIndex then
  begin
    if (NewIndex < 0) or (NewIndex >= Count) then
      Error(SListIndexError, NewIndex);
    Item := Get(CurIndex);
    FList.RemoveAt(CurIndex);
    FList.Insert(NewIndex, Item);
  end;
end;

procedure TIdNetList.Put(Index: Integer; Item: &Object);
var
  Temp: &Object;
begin
  if (Index < 0) or (Index >= Count) then
    Error(SListIndexError, Index);
  if Item <> FList[Index] then
  begin
    Temp := FList[Index];
    FList[Index] := Item;
    if Temp <> nil then
      Notify(Temp, lnDeleted);
    if Item <> nil then
      Notify(Item, lnAdded);
  end;
end;

function TIdNetList.Remove(Item: &Object): Integer;
begin
  Result := IndexOf(Item);
  if Result >= 0 then
    Delete(Result);
end;

procedure TIdNetList.Pack;
var
  I: Integer;
begin
  for I := Count - 1 downto 0 do
    if Items[I] = nil then
      Delete(I);
end;

procedure TIdNetList.SetCapacity(NewCapacity: Integer);
begin
  if NewCapacity < Count then
    Error(SListCapacityError, NewCapacity);
  FList.Capacity := NewCapacity;
end;

procedure TIdNetList.SetCount(NewCount: Integer);
var
  I, C: Integer;
  TempArray: array of System.Object;
begin
  if NewCount < 0 then
    Error(SListCountError, NewCount);
  C := FList.Count;
  if NewCount > C then
  begin
    SetLength(TempArray, NewCount - C);
    FList.AddRange(System.Object(TempArray) as ICollection);
  end
  else
  begin
    SetLength(TempArray, C - NewCount);
    FList.CopyTo(TempArray, NewCount);
    FList.RemoveRange(NewCount, C - NewCount);
    for I := 0 to Length(TempArray) - 1 do
      Notify(TempArray[I], lnDeleted);
  end;
end;

procedure TIdNetList.Sort(Compare: TIdNetListSortCompare);
begin
  FList.Sort(TIdNetListComparer.Create(Compare));
end;

function TIdNetList.Extract(Item: &Object): &Object;
var
  I: Integer;
begin
  Result := nil;
  I := IndexOf(Item);
  if I >= 0 then
  begin
    Result := Item;
    FList.RemoveAt(I);
    Notify(Result, lnExtracted);
  end;
end;

procedure TIdNetList.Notify(Instance: &Object; Action: TIdNetListNotification);
begin
end;

procedure TIdNetList.Assign(ListA: TIdNetList; AOperator: TIdNetListAssignOp; ListB: TIdNetList);
var
  I: Integer;
  LTemp, LSource: TIdNetList;
begin
  // ListB given?
  if ListB <> nil then
  begin
    LSource := ListB;
    Assign(ListA);
  end
  else
    LSource := ListA;

  // on with the show
  case AOperator of

    // 12345, 346 = 346 : only those in the new list
    laCopy:
      begin
        Clear;
        Capacity := LSource.Capacity;
        for I := 0 to LSource.Count - 1 do
          Add(LSource[I]);
      end;

    // 12345, 346 = 34 : intersection of the two lists
    laAnd:
      for I := Count - 1 downto 0 do
        if LSource.IndexOf(Items[I]) = -1 then
          Delete(I);

    // 12345, 346 = 123456 : union of the two lists
    laOr:
      for I := 0 to LSource.Count - 1 do
        if IndexOf(LSource[I]) = -1 then
          Add(LSource[I]);

    // 12345, 346 = 1256 : only those not in both lists
    laXor:
      begin
        LTemp := TIdNetList.Create; // Temp holder of 4 byte values
        LTemp.Capacity := LSource.Count;
        for I := 0 to LSource.Count - 1 do
          if IndexOf(LSource[I]) = -1 then
            LTemp.Add(LSource[I]);
        for I := Count - 1 downto 0 do
          if LSource.IndexOf(Items[I]) <> -1 then
            Delete(I);
        I := Count + LTemp.Count;
        if Capacity < I then
          Capacity := I;
        for I := 0 to LTemp.Count - 1 do
          Add(LTemp[I]);
      end;

    // 12345, 346 = 125 : only those unique to source
    laSrcUnique:
      for I := Count - 1 downto 0 do
        if LSource.IndexOf(Items[I]) <> -1 then
          Delete(I);

    // 12345, 346 = 6 : only those unique to dest
    laDestUnique:
      begin
        LTemp := TIdNetList.Create;
        LTemp.Capacity := LSource.Count;
        for I := LSource.Count - 1 downto 0 do
          if IndexOf(LSource[I]) = -1 then
            LTemp.Add(LSource[I]);
        Assign(LTemp);
      end;
  end;
end;

{ TIdNetListEnumerator }

constructor TIdNetListEnumerator.Create(AList: TIdNetList);
begin
  inherited Create;
  FIndex := -1;
  FList := AList;
end;

function TIdNetListEnumerator.GetCurrent: &Object;
begin
  Result := FList[FIndex];
end;

function TIdNetListEnumerator.MoveNext: Boolean;
begin
  Result := FIndex < FList.Count - 1;
  if Result then
    Inc(FIndex);
end;

{ TCollectionItem }

constructor TIdNetCollectionItem.Create(Collection: TIdNetCollection);
begin
  inherited Create;
  SetCollection(Collection);
end;

destructor TIdNetCollectionItem.Destroy;
begin
  SetCollection(nil);
  inherited;
end;

procedure TIdNetCollectionItem.Changed(AllItems: Boolean);
var
  Item: TIdNetCollectionItem;
begin
  if (FCollection <> nil) and (FCollection.FUpdateCount = 0) then
  begin
    if AllItems then
      Item := nil
    else
      Item := Self;
    FCollection.Update(Item);
  end;
end;

function TIdNetCollectionItem.GetIndex: Integer;
begin
  if FCollection <> nil then
    Result := FCollection.FItems.IndexOf(Self)
  else
    Result := -1;
end;

function TIdNetCollectionItem.GetDisplayName: string;
begin
  Result := ClassName;
end;

function TIdNetCollectionItem.GetOwner: TIdNetPersistent;
begin
  Result := FCollection;
end;

procedure TIdNetCollectionItem.SetCollection(Value: TIdNetCollection);
begin
  if FCollection <> Value then
  begin
    if FCollection <> nil then
      FCollection.RemoveItem(Self);
    if Value <> nil then
      Value.InsertItem(Self);
  end;
end;

procedure TIdNetCollectionItem.SetDisplayName(const Value: string);
begin
  Changed(False);
end;

procedure TIdNetCollectionItem.SetIndex(Value: Integer);
var
  CurIndex: Integer;
begin
  CurIndex := GetIndex;
  if (CurIndex >= 0) and (CurIndex <> Value) then
  begin
    FCollection.FItems.Move(CurIndex, Value);
    Changed(True);
  end;
end;

{ TCollectionEnumerator }

constructor TIdNetCollectionEnumerator.Create(ACollection: TIdNetCollection);
begin
  inherited Create;
  FIndex := -1;
  FCollection := ACollection;
end;

function TIdNetCollectionEnumerator.GetCurrent: TIdNetCollectionItem;
begin
  Result := FCollection.Items[FIndex];
end;

function TIdNetCollectionEnumerator.MoveNext: Boolean;
begin
  Result := FIndex < FCollection.Count - 1;
  if Result then
    Inc(FIndex);
end;

{ TCollection }

constructor TIdNetCollection.Create(ItemClass: TIdNetCollectionItemClass);
begin
  inherited Create;
  FItemClass := ItemClass;
  FItems := TIdNetList.Create;
end;

destructor TIdNetCollection.Destroy;
begin
  FUpdateCount := 1;
  inherited Destroy;
end;

function TIdNetCollection.Add: TIdNetCollectionItem;
begin
  Result := FItemClass.Create(Self);
end;

procedure TIdNetCollection.BeginUpdate;
begin
  Inc(FUpdateCount);
end;

procedure TIdNetCollection.Changed;
begin
  if FUpdateCount = 0 then
    Update(nil);
end;

procedure TIdNetCollection.Clear;
begin
  if FItems.Count > 0 then
  begin
    BeginUpdate;
    try
      while FItems.Count > 0 do
        TIdNetCollectionItem(FItems.Last).Free;
    finally
      EndUpdate;
    end;
  end;
end;

procedure TIdNetCollection.EndUpdate;
begin
  Dec(FUpdateCount);
  Changed;
end;

function TIdNetCollection.FindItemID(ID: Integer): TIdNetCollectionItem;
var
  I: Integer;
begin
  for I := 0 to FItems.Count-1 do
  begin
    Result := TIdNetCollectionItem(FItems[I]);
    if Result.ID = ID then
      Exit;
  end;
  Result := nil;
end;

function TIdNetCollection.GetCount: Integer;
begin
  Result := FItems.Count;
end;

function TIdNetCollection.GetEnumerator: TIdNetCollectionEnumerator;
begin
  Result := TIdNetCollectionEnumerator.Create(Self);
end;

function TIdNetCollection.GetOwner: TIdNetPersistent;
begin
  Result := nil;
end;            

function TIdNetCollection.GetItem(Index: Integer): TIdNetCollectionItem;
begin
  Result := TIdNetCollectionItem(FItems[Index]);
end;

function TIdNetCollection.Insert(Index: Integer): TIdNetCollectionItem;
begin
  Result := Add;
  Result.Index := Index;
end;

procedure TIdNetCollection.InsertItem(Item: TIdNetCollectionItem);
begin
  if not (Item is FItemClass) then
    TIdNetList.Error(SInvalidProperty, 0);
  FItems.Add(Item);
  Item.FCollection := Self;
  Item.FID := FNextID;
  Inc(FNextID);
  SetItemName(Item);
  Notify(Item, cnAdded);
  Changed;
end;

procedure TIdNetCollection.RemoveItem(Item: TIdNetCollectionItem);
begin
  Notify(Item, cnExtracting);
  if Item = FItems.Last then
    FItems.Delete(FItems.Count - 1)
  else
    FItems.Remove(Item);
  Item.FCollection := nil;
  Changed;
end;

procedure TIdNetCollection.SetItem(Index: Integer; Value: TIdNetCollectionItem);
begin
  FItems[Index] := Value;
end;

procedure TIdNetCollection.SetItemName(Item: TIdNetCollectionItem);
begin
end;

procedure TIdNetCollection.Update(Item: TIdNetCollectionItem);
begin
end;

procedure TIdNetCollection.Delete(Index: Integer);
begin
  Notify(TIdNetCollectionItem(FItems[Index]), cnDeleting);
  TIdNetCollectionItem(FItems[Index]).Free;
end;

procedure TIdNetCollection.Notify(Item: TIdNetCollectionItem;
  Action: TIdNetCollectionNotification);
begin
end;

{ TIdNetThreadRunner }

type
  TIdNetThreadRunner = class
    FThread: TIdNetThread;
  public
    constructor Create(AThread: TIdNetThread);
    procedure ThreadProc;
    procedure Initialize; virtual;
  end;

constructor TIdNetThreadRunner.Create(AThread: TIdNetThread);
begin
  inherited Create;
  FThread := AThread;
end;

procedure TIdNetThreadRunner.ThreadProc;
var
  FreeThread: Boolean;
begin
  try
    if not FThread.Terminated then
    try
      Initialize;
      FThread.Execute;
    except
      on E: Exception do
        FThread.FFatalException := E;
    end;
  finally
    FreeThread := FThread.FFreeOnTerminate;
    FThread.DoTerminate;
    FThread.FFinished := True;
    SignalSyncEvent;
    if FreeThread then FThread.Free;
  end;
end;

procedure TIdNetThreadRunner.Initialize;
begin
end;

{ TIdNetThread }

constructor TIdNetThread.Create(CreateSuspended: Boolean);
begin
  inherited Create;
//  AddThread;
  FSuspended := CreateSuspended;
  FCreateSuspended := CreateSuspended;
  Initialize;
end;

procedure TIdNetThread.Initialize;
var
  Runner: TIdNetThreadRunner;
begin
  Runner := TIdNetThreadRunner.Create(self);
  FHandle := System.Threading.Thread.Create(@Runner.ThreadProc);
  if not FCreateSuspended then
  begin
    FStarted := True;
    FHandle.Start;
  end
  else
    FSuspendCount := 1;
end;

destructor TIdNetThread.Destroy;
begin
  if (FHandle <> nil) and FStarted and not FFinished then
  begin
    Terminate;
    if not FHandle.IsAlive then
      Resume;
    WaitFor;
  end;
  RemoveQueuedEvents(Self, nil);
  FHandle := nil;
  FFatalException.Free;
  inherited Destroy;
//  RemoveThread;
end;

procedure TIdNetThread.ThreadError(O: TObject);
var
  S: string;
begin
  if Assigned(O) then
  begin
    if O is Exception then
      S := Exception(O).Message
    else
      S := O.ToString;
    raise EThread.Create(Sys.Format(SThreadError, [S]));
  end;
end;

procedure TIdNetThread.CallOnTerminate;
begin
  if Assigned(FOnTerminate) then FOnTerminate(Self);
end;

procedure TIdNetThread.DoTerminate;
begin
  if Assigned(FOnTerminate) then CallOnTerminate;
end;

const
  Priorities: array [TIdNetThreadPriority] of System.Threading.ThreadPriority =
   (System.Threading.ThreadPriority.Lowest,
    System.Threading.ThreadPriority.BelowNormal,
    System.Threading.ThreadPriority.Normal,
    System.Threading.ThreadPriority.AboveNormal,
    System.Threading.ThreadPriority.Highest);

function TIdNetThread.GetPriority: TIdNetThreadPriority;
var
  P: System.Threading.ThreadPriority;
  I: TIdNetThreadPriority;
begin
  Result := TIdNetThreadPriority.Normal;
  try
    P := FHandle.Priority;

    for I := Low(TIdNetThreadPriority) to High(TIdNetThreadPriority) do
      if Priorities[I] = P then
      begin
        Result := I;
        Break;
      end;
  except
    on E: Exception do
      ThreadError(E);
  end;
end;

procedure TIdNetThread.SetPriority(Value: TIdNetThreadPriority);
begin
  try
    FHandle.Priority := Priorities[Value];
  except
    on E: Exception do
      ThreadError(E);
  end;
end;

procedure TIdNetThread.Queue(AMethod: TIdNetThreadMethod);
begin
  AMethod;
end;

class procedure TIdNetThread.Queue(AThread: TIdNetThread; AMethod: TIdNetThreadMethod);
begin
  AMethod;
end;

class procedure TIdNetThread.RemoveQueuedEvents(AThread: TIdNetThread; AMethod: TIdNetThreadMethod);
var
  I: Integer;
  SyncProc: TIdNetSyncProc;
begin
  System.Threading.Monitor.Enter(ThreadLock);
  try
    if SyncList <> nil then
      for I := SyncList.Count - 1 downto 0 do
      begin
        SyncProc := TIdNetSyncProc(SyncList[I]);
        if (SyncProc.Signal = nil) and
          (((AThread <> nil) and (SyncProc.SyncRec.FThread = AThread)) or
            (Assigned(AMethod) and TObject(@AMethod).Equals(TObject(@SyncProc.SyncRec.FMethod)))) then
          SyncList.Delete(I);
      end;
  finally
    System.Threading.Monitor.Exit(ThreadLock);
  end;
end;

class procedure TIdNetThread.StaticQueue(AThread: TIdNetThread; AMethod: TIdNetThreadMethod);
begin
  AMethod;
end;

procedure TIdNetThread.Synchronize(Method: TIdNetThreadMethod);
begin
  Method;
end;

procedure TIdNetThread.SetSuspended(Value: Boolean);
begin
  if Value <> FSuspended then
    if Value then
      Suspend
    else
      Resume;
end;

procedure TIdNetThread.Suspend;
var
  OldSuspend: Boolean;
begin
  OldSuspend := FSuspended;
  try
    System.Threading.Monitor.Enter(self);
    try
      FSuspended := True;
      FHandle.Suspend;
      Inc(FSuspendCount);
    finally
      System.Threading.Monitor.Exit(self);
    end;
  except
    on E: Exception do
    begin
      FSuspended := OldSuspend;
      ThreadError(E);
    end;
  end;
end;

procedure TIdNetThread.Resume;
begin
  if FSuspendCount > 0 then
  begin
    System.Threading.Monitor.Enter(self);
    try
      Dec(FSuspendCount);
      if FSuspendCount = 0 then
      begin
        if not FStarted then
          FHandle.Start
        else
          FHandle.Resume;
        FSuspended := False;
      end;
    finally
      System.Threading.Monitor.Exit(self);
    end;
  end;
end;

procedure TIdNetThread.Terminate;
begin
  FTerminated := True;
end;

function TIdNetThread.WaitFor: LongWord;
begin
  WaitFor(System.Threading.Timeout.Infinite, Result);
end;

function TIdNetThread.WaitFor(TimeOut: Integer; var ReturnValue: LongWord): Boolean;
begin
  Result := False;
  try
    if (TimeOut = System.Threading.Timeout.Infinite) and
       (System.Threading.Thread.CurrentThread = MainThread) then
    begin
      while True do
      begin
      { This prevents a potential deadlock if the background thread
        does a Synchronize to the foreground thread }
        Result := FHandle.Join(500);
        if Result then
        begin
          ReturnValue := FReturnValue;
          Exit;
        end;
        Assert(SyncEvent<>nil);
        if SyncEvent.WaitOne(500, True) then
          CheckSynchronize;
      end
    end else
    begin
      Result := FHandle.Join(Timeout);
      if Result then
        ReturnValue := FReturnValue;
    end;
  except
    on E: Exception do
      ThreadError(E);
  end;
end;

{ TThreadList }

constructor TIdNetThreadList.Create;
begin
  inherited Create;
  FList := TIdNetList.Create;
  FDuplicates := dupIgnore;
end;

procedure TIdNetThreadList.Add(Item: &Object);
begin
  LockList;
  try
    if (Duplicates = dupAccept) or
       (FList.IndexOf(Item) = -1) then
      FList.Add(Item)
    else if Duplicates = dupError then
      FList.Error(SDuplicateItem, Integer(Item));
  finally
    UnlockList;
  end;
end;

procedure TIdNetThreadList.Clear;
begin
  LockList;
  try
    FList.Clear;
  finally
    UnlockList;
  end;
end;

function  TIdNetThreadList.LockList: TIdNetList;
begin
  System.Threading.Monitor.Enter(Self);
  Result := FList;
end;

procedure TIdNetThreadList.Remove(Item: &Object);
begin
  LockList;
  try
    FList.Remove(Item);
  finally
    UnlockList;
  end;
end;

procedure TIdNetThreadList.UnlockList;
begin
  System.Threading.Monitor.Exit(Self);
end;

{ TIdNetOwnedCollection }

constructor TIdNetOwnedCollection.Create(AOwner: TIdNetPersistent;
  ItemClass: TIdNetCollectionItemClass);
begin
  inherited Create(ItemClass);
  FOwner := AOwner;
end;

function TIdNetOwnedCollection.GetOwner: TIdNetPersistent;
begin
  Result := FOwner;
end;

procedure TIdNetCollectionItem.Assign(ASource: TIdNetPersistent);
begin
  inherited Assign(ASource);
end;

{ TIdNetPersistentHelper }

constructor TIdNetPersistentHelper.Create;
begin
  inherited Create;
  InsideCreate;
end;

procedure TIdNetPersistentHelper.Assign(ASource: TIdNetPersistent);
begin
  if ASource <> nil then
    ASource.AssignTo(Self);
end;

procedure TIdNetPersistentHelper.AssignTo(Dest: TIdNetPersistent);
begin

end;

function TIdNetPersistentHelper.GetNamePath: string;
var
  S: string;
begin
  Result := ClassName;
  if GetOwner <> nil then
  begin
    S := GetOwner.GetNamePath;
    if S <> '' then
      Result := S + '.' + Result;
  end;
end;

function TIdNetPersistentHelper.GetOwner: TIdNetPersistent;
begin
  Result := nil;
end;

{ TIdNetMultiReadExclusiveWriteSynchronizer }

constructor TIdNetMultiReadExclusiveWriteSynchronizer.Create;
begin
  inherited;
  FReaderWriterLock := System.Threading.ReaderWriterLock.Create;
end;

function TIdNetMultiReadExclusiveWriteSynchronizer.GetRevisionLevel: Integer;
begin
  Result := FReaderWriterLock.WriterSeqNum;
end;

procedure TIdNetMultiReadExclusiveWriteSynchronizer.BeginRead;
begin
  FReaderWriterLock.AcquireReaderLock(-1);
end;

procedure TIdNetMultiReadExclusiveWriteSynchronizer.EndRead;
begin
  FReaderWriterLock.ReleaseReaderLock;
end;

function TIdNetMultiReadExclusiveWriteSynchronizer.BeginWrite: Boolean;
begin
  FReaderWriterLock.AcquireWriterLock(-1);
  Result := FReaderWriterLock.IsWriterLockHeld;
end;

procedure TIdNetMultiReadExclusiveWriteSynchronizer.EndWrite;
begin
  FReaderWriterLock.ReleaseWriterLock;
end;

{ TIdNetNativeComponentHelper }

function TIdNetNativeComponent.GetName: string;
begin
  Result := GetSiteObject.FName;
end;

procedure TIdNetNativeComponent.SetName(const Value: string);
begin
  GetSiteObject.FName := Value;
end;

function TIdNetNativeComponent.GetTag: &Object;
begin
  Result := GetSiteObject.FTag;
end;

procedure TIdNetNativeComponent.SetTag(const Value: &Object);
begin
  GetSiteObject.FTag := Value;
end;

function TIdNetNativeComponent.GetSiteObject: TIdNetNativeComponentSite;
begin
  if Site = nil then
    Site := TIdNetNativeComponentSite.Create(Self, nil);
  if TObject(Site) is TIdNetNativeComponentSite then
    Result := &Object(Site) as TIdNetNativeComponentSite
  else
    Result := TIdNetNativeComponentSite.Create(Self, nil);
end;

{ TIdNetNativeComponent }

procedure TIdNetNativeComponent.EndInit;
begin
  Exclude(FComponentState, csLoading);
  Loaded;
end;

procedure TIdNetNativeComponent.BeginInit;
begin
  Include(FComponentState, csLoading);
end;

function TIdNetNativeComponent.GetComponentState: TIdNetNativeComponentState;
begin
  Result := FComponentState;
end;

procedure TIdNetNativeComponent.Notification(AComponent: TIdNetNativeComponent;
  Operation: TIdNetNativeOperation);
var
  I: Integer;
begin
  if (Operation = opRemove) and (AComponent <> nil) then
    RemoveFreeNotification(AComponent);
  with GetSiteObject do
    if FComponents <> nil then
    begin
      I := FComponents.Count - 1;
      while I >= 0 do
      begin
        TIdNetNativeComponent(FComponents[I]).Notification(AComponent, Operation);
        Dec(I);
        if I >= FComponents.Count then
          I := FComponents.Count - 1;
      end;
    end;
end;

procedure TIdNetNativeComponent.RemoveFreeNotification(
  AComponent: TIdNetNativeComponent);
begin
  RemoveNotification(AComponent);
  AComponent.RemoveNotification(Self);
end;

procedure TIdNetNativeComponent.FreeNotification(
  AComponent: TIdNetNativeComponent);
begin
  if (Owner = nil) or (AComponent.Owner <> Owner) then
  begin
    // Never acquire a reference to a component that is being deleted.
    assert(not (csDestroying in (ComponentState + AComponent.ComponentState)));
    if not Assigned(FFreeNotifies) then
      FFreeNotifies := TIdNetList.Create;
    if FFreeNotifies.IndexOf(AComponent) < 0 then
    begin
      FFreeNotifies.Add(AComponent);
      AComponent.FreeNotification(Self);
    end;
  end;
  FComponentState := FComponentState + [csFreeNotification];
end;

procedure TIdNetNativeComponent.RemoveNotification(AComponent: TIdNetNativeComponent);
begin
  if FFreeNotifies <> nil then
  begin
    FFreeNotifies.Remove(AComponent);
    if FFreeNotifies.Count = 0 then
    begin
      FFreeNotifies.Free;
      FFreeNotifies := nil;
    end;
  end;
end;

procedure TIdNetNativeComponent.Loaded;
begin

end;

{ TIdNetNativeComponentSite }

constructor TIdNetNativeComponentSite.Create(AInstance,
  AOwner: TIdNetNativeComponent);
begin
  inherited Create;
  if Assigned(AInstance) then
    FComponent := AInstance as IComponent
  else
    FComponent := nil;
  FOwner := AOwner;
  FName := '';
  FDesignMode := False;
end;

function TIdNetNativeComponent.GetSelfOwner: TIdNetNativeComponent;
begin
  Result := TIdNetNativeComponent(TIdNetNativeComponent(Self).GetOwner);
end;

function TIdNetNativeComponentSite.GetService(AType: System.Type): &Object;
begin
  Result := nil;
end;

function TIdNetNativeComponentSite.get_Container: IContainer;
begin
  if (FOwner <> nil) and (FOwner.Site is IContainer) then
    Result := FOwner.Site as IContainer
  else
    Result := nil;
end;

procedure TIdNetPersistentHelper.InsideCreate;
begin
end;

constructor TIdNetPersistentHelper.Create(AOwner: TIdNetNativeComponent);
begin
  inherited Create;
end;

procedure TIdNetNativeComponent.InsideCreate;
begin
  inherited InsideCreate;
  FComponentState := [];
end;

function TIdNetNativeComponent.GetOwner: TIdNetPersistent;
begin
  Result := FOwner;
end;

function TIdStringListFCL.AddObject(S: string; AObject: &Object): Integer;
begin
  if IndexOf(S) > -1 then
  begin
    case Duplicates of
      dupIgnore: Exit;
      dupError: raise Exception.Create('Duplicate items in stringlist not allowed!');
    end;
  end;
  Result := inherited AddObject(S, AObject);
end;

constructor TIdNetNativeComponentHelper.Create(AOwner: TIdNetNativeComponent);
begin
  inherited Create;
  FOwner := AOwner;
end;

initialization
	InitThreadSynchronization;
finalization
	DoneThreadSynchronization;
end.