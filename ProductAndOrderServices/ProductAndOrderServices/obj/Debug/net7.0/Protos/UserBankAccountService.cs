// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/user_bank_account_service.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace UserAndBankAccountServices {

  /// <summary>Holder for reflection information generated from Protos/user_bank_account_service.proto</summary>
  public static partial class UserBankAccountServiceReflection {

    #region Descriptor
    /// <summary>File descriptor for Protos/user_bank_account_service.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static UserBankAccountServiceReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CiZQcm90b3MvdXNlcl9iYW5rX2FjY291bnRfc2VydmljZS5wcm90bxISdXNl",
            "ckFuZEJhbmtBY2NvdW50Ghtnb29nbGUvcHJvdG9idWYvZW1wdHkucHJvdG8i",
            "XQoPVHJhbnNmZXJSZXF1ZXN0EhsKE3NlbmRlckJhbmtBY2NvdW50SWQYASAB",
            "KAkSHQoVcmVjaWV2ZXJCYW5rQWNjb3VudElkGAIgASgJEg4KBmFtb3VudBgD",
            "IAEoASIpChdHZXRCYW5rQWNjb3VudElkUmVxdWVzdBIOCgZ1c2VySWQYASAB",
            "KAkiLgoVQmFua0FjY291bnRJZFJlc3BvbnNlEhUKDWJhbmtBY2NvdW50SWQY",
            "ASABKAky1QEKGlVzZXJBbmRCYW5rQWNjb3VudFNlcnZpY2VzEkkKCFRyYW5z",
            "ZmVyEiMudXNlckFuZEJhbmtBY2NvdW50LlRyYW5zZmVyUmVxdWVzdBoWLmdv",
            "b2dsZS5wcm90b2J1Zi5FbXB0eSIAEmwKEEdldEJhbmtBY2NvdW50SWQSKy51",
            "c2VyQW5kQmFua0FjY291bnQuR2V0QmFua0FjY291bnRJZFJlcXVlc3QaKS51",
            "c2VyQW5kQmFua0FjY291bnQuQmFua0FjY291bnRJZFJlc3BvbnNlIgBCHaoC",
            "GlVzZXJBbmRCYW5rQWNjb3VudFNlcnZpY2VzYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Google.Protobuf.WellKnownTypes.EmptyReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::UserAndBankAccountServices.TransferRequest), global::UserAndBankAccountServices.TransferRequest.Parser, new[]{ "SenderBankAccountId", "RecieverBankAccountId", "Amount" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::UserAndBankAccountServices.GetBankAccountIdRequest), global::UserAndBankAccountServices.GetBankAccountIdRequest.Parser, new[]{ "UserId" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::UserAndBankAccountServices.BankAccountIdResponse), global::UserAndBankAccountServices.BankAccountIdResponse.Parser, new[]{ "BankAccountId" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class TransferRequest : pb::IMessage<TransferRequest>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<TransferRequest> _parser = new pb::MessageParser<TransferRequest>(() => new TransferRequest());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<TransferRequest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::UserAndBankAccountServices.UserBankAccountServiceReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public TransferRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public TransferRequest(TransferRequest other) : this() {
      senderBankAccountId_ = other.senderBankAccountId_;
      recieverBankAccountId_ = other.recieverBankAccountId_;
      amount_ = other.amount_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public TransferRequest Clone() {
      return new TransferRequest(this);
    }

    /// <summary>Field number for the "senderBankAccountId" field.</summary>
    public const int SenderBankAccountIdFieldNumber = 1;
    private string senderBankAccountId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string SenderBankAccountId {
      get { return senderBankAccountId_; }
      set {
        senderBankAccountId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "recieverBankAccountId" field.</summary>
    public const int RecieverBankAccountIdFieldNumber = 2;
    private string recieverBankAccountId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string RecieverBankAccountId {
      get { return recieverBankAccountId_; }
      set {
        recieverBankAccountId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "amount" field.</summary>
    public const int AmountFieldNumber = 3;
    private double amount_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public double Amount {
      get { return amount_; }
      set {
        amount_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as TransferRequest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(TransferRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (SenderBankAccountId != other.SenderBankAccountId) return false;
      if (RecieverBankAccountId != other.RecieverBankAccountId) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(Amount, other.Amount)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (SenderBankAccountId.Length != 0) hash ^= SenderBankAccountId.GetHashCode();
      if (RecieverBankAccountId.Length != 0) hash ^= RecieverBankAccountId.GetHashCode();
      if (Amount != 0D) hash ^= pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(Amount);
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (SenderBankAccountId.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(SenderBankAccountId);
      }
      if (RecieverBankAccountId.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(RecieverBankAccountId);
      }
      if (Amount != 0D) {
        output.WriteRawTag(25);
        output.WriteDouble(Amount);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (SenderBankAccountId.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(SenderBankAccountId);
      }
      if (RecieverBankAccountId.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(RecieverBankAccountId);
      }
      if (Amount != 0D) {
        output.WriteRawTag(25);
        output.WriteDouble(Amount);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (SenderBankAccountId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(SenderBankAccountId);
      }
      if (RecieverBankAccountId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(RecieverBankAccountId);
      }
      if (Amount != 0D) {
        size += 1 + 8;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(TransferRequest other) {
      if (other == null) {
        return;
      }
      if (other.SenderBankAccountId.Length != 0) {
        SenderBankAccountId = other.SenderBankAccountId;
      }
      if (other.RecieverBankAccountId.Length != 0) {
        RecieverBankAccountId = other.RecieverBankAccountId;
      }
      if (other.Amount != 0D) {
        Amount = other.Amount;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            SenderBankAccountId = input.ReadString();
            break;
          }
          case 18: {
            RecieverBankAccountId = input.ReadString();
            break;
          }
          case 25: {
            Amount = input.ReadDouble();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            SenderBankAccountId = input.ReadString();
            break;
          }
          case 18: {
            RecieverBankAccountId = input.ReadString();
            break;
          }
          case 25: {
            Amount = input.ReadDouble();
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class GetBankAccountIdRequest : pb::IMessage<GetBankAccountIdRequest>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<GetBankAccountIdRequest> _parser = new pb::MessageParser<GetBankAccountIdRequest>(() => new GetBankAccountIdRequest());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<GetBankAccountIdRequest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::UserAndBankAccountServices.UserBankAccountServiceReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public GetBankAccountIdRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public GetBankAccountIdRequest(GetBankAccountIdRequest other) : this() {
      userId_ = other.userId_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public GetBankAccountIdRequest Clone() {
      return new GetBankAccountIdRequest(this);
    }

    /// <summary>Field number for the "userId" field.</summary>
    public const int UserIdFieldNumber = 1;
    private string userId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string UserId {
      get { return userId_; }
      set {
        userId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as GetBankAccountIdRequest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(GetBankAccountIdRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (UserId != other.UserId) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (UserId.Length != 0) hash ^= UserId.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (UserId.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(UserId);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (UserId.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(UserId);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (UserId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(UserId);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(GetBankAccountIdRequest other) {
      if (other == null) {
        return;
      }
      if (other.UserId.Length != 0) {
        UserId = other.UserId;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            UserId = input.ReadString();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            UserId = input.ReadString();
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class BankAccountIdResponse : pb::IMessage<BankAccountIdResponse>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<BankAccountIdResponse> _parser = new pb::MessageParser<BankAccountIdResponse>(() => new BankAccountIdResponse());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<BankAccountIdResponse> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::UserAndBankAccountServices.UserBankAccountServiceReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public BankAccountIdResponse() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public BankAccountIdResponse(BankAccountIdResponse other) : this() {
      bankAccountId_ = other.bankAccountId_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public BankAccountIdResponse Clone() {
      return new BankAccountIdResponse(this);
    }

    /// <summary>Field number for the "bankAccountId" field.</summary>
    public const int BankAccountIdFieldNumber = 1;
    private string bankAccountId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string BankAccountId {
      get { return bankAccountId_; }
      set {
        bankAccountId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as BankAccountIdResponse);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(BankAccountIdResponse other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (BankAccountId != other.BankAccountId) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (BankAccountId.Length != 0) hash ^= BankAccountId.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (BankAccountId.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(BankAccountId);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (BankAccountId.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(BankAccountId);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (BankAccountId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(BankAccountId);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(BankAccountIdResponse other) {
      if (other == null) {
        return;
      }
      if (other.BankAccountId.Length != 0) {
        BankAccountId = other.BankAccountId;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            BankAccountId = input.ReadString();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            BankAccountId = input.ReadString();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
